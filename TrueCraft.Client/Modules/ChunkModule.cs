﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Client.Events;
using TrueCraft.Client.Rendering;
using TrueCraft.Core.Lighting;
using TrueCraft.Core.World;

namespace TrueCraft.Client.Modules
{
    public class ChunkModule : IGraphicalModule
    {
        public TrueCraftGame Game { get; set; }
        public ChunkRenderer ChunkRenderer { get; set; }
        public int ChunksRendered { get; set; }

        private HashSet<GlobalChunkCoordinates> ActiveMeshes { get; }

        private List<ChunkMesh> ChunkMeshes { get; set; }
        private ConcurrentBag<Mesh> IncomingChunks { get; set; }
        private Lighting WorldLighting { get; set; }

        private BasicEffect OpaqueEffect { get; set; }
        private AlphaTestEffect TransparentEffect { get; set; }

        public ChunkModule(TrueCraftGame game)
        {
            Game = game;

            ChunkRenderer = new ChunkRenderer(Game.Client.World, Game, Game.BlockRepository);
            Game.Client.ChunkLoaded += Game_Client_ChunkLoaded;
            Game.Client.ChunkUnloaded += (sender, e) => UnloadChunk(e.Chunk);
            Game.Client.ChunkModified += Game_Client_ChunkModified;
            Game.Client.BlockChanged += Game_Client_BlockChanged;
            ChunkRenderer.MeshCompleted += MeshCompleted;
            ChunkRenderer.Start();
            WorldLighting = new Lighting(Game.Client.World.World, Game.BlockRepository);

            OpaqueEffect = new BasicEffect(Game.GraphicsDevice);
            OpaqueEffect.TextureEnabled = true;
            OpaqueEffect.Texture = Game.TextureMapper.GetTexture("terrain.png");
            OpaqueEffect.FogEnabled = true;
            OpaqueEffect.FogStart = 0;
            OpaqueEffect.FogEnd = Game.Camera.Frustum.Far.D * 0.8f;
            OpaqueEffect.VertexColorEnabled = true;
            OpaqueEffect.LightingEnabled = true;

            TransparentEffect = new AlphaTestEffect(Game.GraphicsDevice);
            TransparentEffect.AlphaFunction = CompareFunction.Greater;
            TransparentEffect.ReferenceAlpha = 127;
            TransparentEffect.Texture = Game.TextureMapper.GetTexture("terrain.png");
            TransparentEffect.VertexColorEnabled = true;
            OpaqueEffect.LightingEnabled = true;

            ChunkMeshes = new List<ChunkMesh>();
            IncomingChunks = new ConcurrentBag<Mesh>();
            ActiveMeshes = new HashSet<GlobalChunkCoordinates>();
        }

        void Game_Client_BlockChanged(object sender, BlockChangeEventArgs e)
        {
            WorldLighting.EnqueueOperation(new TrueCraft.Core.BoundingBox(
                (Core.Vector3)e.Position, (Core.Vector3)(e.Position + Vector3i.One)), false);
            WorldLighting.EnqueueOperation(new TrueCraft.Core.BoundingBox(
                (Core.Vector3)e.Position, (Core.Vector3)(e.Position + Vector3i.One)), true);

            // TODO What is the purpose of enqueuing an entire column after the block?
            //      Could it be related to the lighting height map adding 2 to the height?
            //      That would result in incorrect heights when placing blocks high above other blocks.
            Core.Vector3 posA = new Core.Vector3(e.Position.X, 0, e.Position.Z);
            Core.Vector3 posB = new Core.Vector3(e.Position.X + 1, Dimension.Height, e.Position.Z + 1);
            WorldLighting.EnqueueOperation(new TrueCraft.Core.BoundingBox(posA, posB), true);
            WorldLighting.EnqueueOperation(new TrueCraft.Core.BoundingBox(posA, posB), false);
            for (int i = 0; i < 100; i++)
            {
                if (!WorldLighting.TryLightNext())
                    break;
            }

        }

        private void Game_Client_ChunkModified(object sender, ChunkEventArgs e)
        {
            ChunkRenderer.Enqueue(e.Chunk, true);
        }

        //private readonly static Coordinates2D[] AdjacentCoordinates =
        //    {
        //        Coordinates2D.North, Coordinates2D.South,
        //        Coordinates2D.East, Coordinates2D.West
        //    };

        private void Game_Client_ChunkLoaded(object sender, ChunkEventArgs e)
        {
            ChunkRenderer.Enqueue(e.Chunk);

            //    // TODO:  What is the reason for enqueueing adjacent chunks?
            //    //        Surely, they would have been queued when loaded?
            //    //        NOTE: World.GetChunk WILL load or generate the chunk if not present!!!!!
            //    //              This is expected to cause runaway chunk loading/generating.
            //    for (int i = 0; i < AdjacentCoordinates.Length; i++)
            //    {
            //        ReadOnlyChunk adjacent = Game.Client.World.GetChunk(
            //             AdjacentCoordinates[i] + e.Chunk.Coordinates);
            //        if (adjacent != null)
            //            ChunkRenderer.Enqueue(adjacent);
            //    }
        }

        void MeshCompleted(object sender, RendererEventArgs<ReadOnlyChunk> e)
        {
            IncomingChunks.Add(e.Result);
        }

        void UnloadChunk(ReadOnlyChunk chunk)
        {
            Game.Invoke(() =>
            {
                ActiveMeshes.Remove(chunk.Coordinates);
                ChunkMeshes.RemoveAll(m => m.Chunk.Coordinates == chunk.Coordinates);
            });
        }

        void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position":
                    var sorter = new ChunkRenderer.ChunkSorter(new GlobalVoxelCoordinates(
                        (int)Game.Client.Position.X, 0, (int)Game.Client.Position.Z));
                    Game.Invoke(() => ChunkMeshes.Sort(sorter));
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            var any = false;
            Mesh _mesh;
            while (IncomingChunks.TryTake(out _mesh))
            {
                any = true;
                var mesh = _mesh as ChunkMesh;
                if (ActiveMeshes.Contains(mesh.Chunk.Coordinates))
                {
                    int existing = ChunkMeshes.FindIndex(m => m.Chunk.Coordinates == mesh.Chunk.Coordinates);
                    ChunkMeshes[existing] = mesh;
                }
                else
                {
                    ActiveMeshes.Add(mesh.Chunk.Coordinates);
                    ChunkMeshes.Add(mesh);
                }
            }
            if (any)
                Game.FlushMainThreadActions();
            WorldLighting.TryLightNext();
        }

        private static readonly BlendState ColorWriteDisable = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.None
        };

        public void Draw(GameTime gameTime)
        {
            OpaqueEffect.FogColor = Game.SkyModule.WorldFogColor.ToVector3();
            Game.Camera.ApplyTo(OpaqueEffect);
            Game.Camera.ApplyTo(TransparentEffect);
            OpaqueEffect.AmbientLightColor = TransparentEffect.DiffuseColor = Color.White.ToVector3() 
                * new Microsoft.Xna.Framework.Vector3(0.25f + Game.SkyModule.BrightnessModifier);

            int chunks = 0;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
                {
                    chunks++;
                    ChunkMeshes[i].Draw(OpaqueEffect, 0);
                    if (!ChunkMeshes[i].IsReady || ChunkMeshes[i].Submeshes != 2)
                        Console.WriteLine("Warning: rendered chunk that was not ready");
                }
            }

            Game.GraphicsDevice.BlendState = ColorWriteDisable;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
                    ChunkMeshes[i].Draw(TransparentEffect, 1);
            }

            Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (Game.Camera.Frustum.Intersects(ChunkMeshes[i].BoundingBox))
                    ChunkMeshes[i].Draw(TransparentEffect, 1);
            }

            ChunksRendered = chunks;
        }
    }
}
