﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TrueCraft.Core.Networking;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Entities;

public abstract class Entity : IEntity
{
    private readonly IDimension _dimension;
    private readonly IEntityManager _entityManager;

    private bool _enablePropertyChange = true;

    protected EntityFlags _entityFlags;

    protected Vector3 _position;
    protected Vector3 _velocity;
    protected float _yaw;
    protected float _pitch;

    private bool _despawned = false;

    private readonly Size _size;
    private readonly float _accelerationDueToGravity;
    private readonly float _drag;
    private readonly float _terminalVelocity;

    protected Entity(
        IDimension dimension,
        IEntityManager entityManager,
        Size size,
        float accelerationDueToGravity,
        float drag,
        float terminalVelocity
    )
    {
        _dimension = dimension;
        _entityManager = entityManager;

        EntityID = -1;
        SpawnTime = DateTime.UtcNow;

        _size = size;
        _accelerationDueToGravity = accelerationDueToGravity;
        _drag = drag;
        _terminalVelocity = terminalVelocity;
    }

    public DateTime SpawnTime { get; set; }

    public int EntityID { get; set; }

    /// <inheritdoc />
    public IEntityManager EntityManager => _entityManager;

    /// <inheritdoc />
    public IDimension Dimension => _dimension;

    public virtual Vector3 Position
    {
        get => _position;
        set
        {
            if (_position == value)
            {
                return;
            }

            _position = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BoundingBox));
        }
    }

    public virtual Vector3 Velocity
    {
        get => _velocity;
        set
        {
            if (_velocity == value)
            {
                return;
            }

            _velocity = value;
            OnPropertyChanged();
        }
    }

    public float Yaw
    {
        get => _yaw;
        set
        {
            if (_yaw == value)
            {
                return;
            }

            _yaw = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public float Pitch
    {
        get => _pitch;
        set
        {
            if (_pitch == value)
            {
                return;
            }

            _pitch = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool Despawned
    {
        get => _despawned;
        set
        {
            if (_despawned && !value)
            {
                throw new InvalidOperationException($"Cannot set Despawned back to true from false.");
            }

            if (_despawned == value)
            {
                return;
            }

            _despawned = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public virtual Size Size => _size;

    /// <inheritdoc />
    public BoundingBox BoundingBox
    {
        get
        {
            var hw = _size.Width * 0.5;
            var hd = _size.Depth * 0.5;
            var min = new Vector3(_position.X - hw, _position.Y, _position.Z - hd);
            var max = new Vector3(_position.X + hw, _position.Y + _size.Height, _position.Z + hd);

            return new BoundingBox(min, max);
        }
    }

    /// <inheritdoc />
    public abstract IPacket SpawnPacket { get; }

    /// <inheritdoc />
    public virtual bool SendMetadataToClients => false;

    public virtual EntityFlags EntityFlags
    {
        get => _entityFlags;
        set
        {
            if (_entityFlags == value)
            {
                return;
            }

            _entityFlags = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public virtual MetadataDictionary Metadata
    {
        get
        {
            var dictionary = new MetadataDictionary();
            dictionary[0] = new MetadataByte((byte) EntityFlags);
            dictionary[1] = new MetadataShort(300);

            return dictionary;
        }
    }

    /// <inheritdoc />
    public virtual void Update(IEntityManager entityManager)
    {
        // TODO: Losing health and all that jazz
        if (Position.Y < -50)
        {
            entityManager.DespawnEntity(this);
        }
    }

    /// <inheritdoc />
    public virtual float AccelerationDueToGravity => _accelerationDueToGravity;

    /// <inheritdoc />
    public virtual float Drag => _drag;

    /// <inheritdoc />
    public virtual float TerminalVelocity => _terminalVelocity;

    /// <inheritdoc />
    public virtual void TerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection)
    {
        // By default, no special handling is needed.
    }

    /// <summary>
    /// Enables or disables Property Change notifications.
    /// </summary>
    protected bool EnablePropertyChange
    {
        get => _enablePropertyChange;
        set
        {
            if (value == _enablePropertyChange)
            {
                return;
            }

            _enablePropertyChange = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public virtual bool BeginUpdate()
    {
        EnablePropertyChange = false;

        return true;
    }

    /// <inheritdoc />
    public virtual void EndUpdate(Vector3 newPosition, Vector3 newVelocity)
    {
        var positionChanged = newPosition != _position;
        var velocityChanged = newVelocity != _velocity;

        if (positionChanged)
        {
            _position = newPosition;
        }

        if (velocityChanged)
        {
            _velocity = newVelocity;
        }

        EnablePropertyChange = true;

        if (positionChanged)
        {
            OnPropertyChanged(nameof(Position));
        }

        if (velocityChanged)
        {
            OnPropertyChanged(nameof(Velocity));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected internal virtual void OnPropertyChanged([CallerMemberName] string property = "")
    {
        if (!EnablePropertyChange)
        {
            return;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}