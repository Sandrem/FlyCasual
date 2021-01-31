using Editions;
using Ship;

namespace SquadBuilderNS
{
    public class ShipRecord
    {
        public GenericShip Instance { get; }
        public string ShipName => Instance.ShipInfo.ShipName;
        public string ShipNameCanonical => Instance.ShipTypeCanonical;
        public string ShipNamespace { get; }
        
        public ShipRecord(string shipNamespace)
        {
            ShipNamespace = shipNamespace;

            // shipTypeNameFull must have format like Ship.SecondEdition.XWing.XWing
            string shipTypeNameFull = shipNamespace + shipNamespace.Substring(shipNamespace.LastIndexOf('.'));
            Instance = (GenericShip) System.Activator.CreateInstance(System.Type.GetType(shipTypeNameFull));
            Edition.Current.AdaptShipToRules(Instance);
        }
    }
}
