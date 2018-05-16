using Ship;
using System.Linq;

namespace BoardTools
{
    public class ShipDistanceInfo : GenericShipDistanceInfo
    {
        public ShipDistanceInfo(GenericShip ship1, GenericShip ship2) : base(ship1, ship2)
        {
            CheckRange();
        }

        private void CheckRange()
        {
            FindNearestDistances(Ship1.ShipBase.GetBaseEdges().Values.ToList());
            TryFindPerpendicularDistanceA();
            TryFindPerpendicularDistanceB();
            SetFinalMinDistance();
        }
    }
}
