using BoardTools;
using Ship;
using System.Collections.Generic;

public static class BoardState
{
    public static List<IBoardObject> BoardObjects = new List<IBoardObject>();

    public static void Initialize()
    {
        BoardObjects = new List<IBoardObject>();
    }

    public static bool IsInRange(IBoardObject objectA, IBoardObject objectB, int minRange = 0, int maxRange = 3)
    {
        if (objectA.BoardObjectType == BoardObjectType.Ship && objectB.BoardObjectType == BoardObjectType.Ship)
        {
            DistanceInfo distInfo = new DistanceInfo(objectA as GenericShip, objectB as GenericShip);
            return distInfo.Range >= minRange && distInfo.Range <= maxRange;
        }
        else
        {
            ColliderDistanceInfo distInfo = new ColliderDistanceInfo(objectA, objectB);
            return distInfo.Range >= minRange && distInfo.Range <= maxRange;
        }
    }

    public static List<IBoardObject> GetObjectsInRange(IBoardObject objectTarget, int minRange = 0, int maxRange = 3, List<BoardObjectType> types = null)
    {
        // TODO: Register object first

        if (types == null) types = new List<BoardObjectType>()
        {
            BoardObjectType.Ship,
            BoardObjectType.Remote,
            BoardObjectType.Obstacle,
            BoardObjectType.Device
        };

        List<IBoardObject> result = new List<IBoardObject>();
        foreach (IBoardObject boardObject in BoardObjects)
        {
            if (types.Contains(boardObject.BoardObjectType) && IsInRange(objectTarget, boardObject, minRange, maxRange))
            {
                result.Add(boardObject);
            }
        }
        return result;
    }

    public static bool IsInArc(IBoardObject objectWithArc, IBoardObject objectTarget, int minRange = 0, int maxRange = 3)
    {
        // TODO
        return IsInRange(objectWithArc, objectTarget, minRange, maxRange);
    }
}
