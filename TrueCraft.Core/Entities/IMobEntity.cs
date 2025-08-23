using System;
using TrueCraft.Core.AI;

namespace TrueCraft.Core.Entities
{
    public interface IMobEntity : IEntity
    {
        event EventHandler PathComplete;
        PathResult? CurrentPath { get; set; }
        bool AdvancePath(TimeSpan time, bool faceRoute = true);
        IMobState? CurrentState { get; set; }
        void Face(Vector3 target);
    }
}
