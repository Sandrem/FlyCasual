using UnityEngine;

public enum BoardObjectType
{
    Ship,
    Remote,
    Obstacle,
    Device
}

public interface IBoardObject
{
    public MeshCollider Collider { get; }
    public BoardObjectType BoardObjectType { get; }
}
