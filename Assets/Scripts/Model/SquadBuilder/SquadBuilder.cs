using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Players;
using Ship;
using System.Reflection;

namespace SquadBuilderNS
{
    public class SquadBuilderShip
    {
        public List<SquadBuilderUpgrade> InstalledUpgrades;
        public GenericShip Instance;
        public SquadList List;

        public SquadBuilderShip(PilotRecord pilotRecord, SquadList list)
        {
            Instance = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
            List = list;
        }
    }

    public class SquadBuilderUpgrade
    {

    }

    public class SquadList
    {
        private List<SquadBuilderShip> Ships;
        public Faction SquadFaction;

        public void AddShip(PilotRecord pilotRecord)
        {
            if (Ships == null) Ships = new List<SquadBuilderShip>();
            Ships.Add(new SquadBuilderShip(pilotRecord, this));
        }

        public List<SquadBuilderShip> GetShips()
        {
            if (Ships == null) Ships = new List<SquadBuilderShip>();
            return Ships;
        }
    }

    public class ShipRecord
    {
        public string ShipName;
        public string ShipNameCanonical;
        public string ShipNamespace;
        public GenericShip Instance;
    }

    public class PilotRecord
    {
        public string PilotName;
        public string PilotTypeName;
        public string PilotNameCanonical;
        public int PilotSkill;
        public ShipRecord PilotShip;
        public GenericShip Instance;
    }

    static partial class SquadBuilder
    {
        public static PlayerNo CurrentPlayer;
        public static Dictionary<PlayerNo, SquadList> SquadLists;
        public static SquadList CurrentSquadList
        {
            get { return SquadLists[CurrentPlayer]; }
        }


        public static List<ShipRecord> AllShips;
        public static List<PilotRecord> AllPilots;

        public static string CurrentShipToBrowsePilots;

        public static void Initialize()
        {
            CurrentPlayer = PlayerNo.Player1;

            SquadLists = new Dictionary<PlayerNo, SquadList>()
            {
                { PlayerNo.Player1, new SquadList() },
                { PlayerNo.Player2, new SquadList() }
            };
            GenerateListOfShips();
        }

        private static void GenerateListOfShips()
        {
            AllShips = new List<ShipRecord>();
            AllPilots = new List<PilotRecord>();

            IEnumerable<string> namespaceIEnum =
                from types in Assembly.GetExecutingAssembly().GetTypes()
                where types.Namespace != null
                where types.Namespace.StartsWith("Ship.")
                select types.Namespace;

            List<string> namespaceList = new List<string>();
            foreach (var ns in namespaceIEnum)
            {
                if (!namespaceList.Contains(ns))
                {
                    namespaceList.Add(ns);
                    GenericShip newShipTypeContainer = (GenericShip)System.Activator.CreateInstance(System.Type.GetType(ns + "." + ns.Substring(5)));

                    if (AllShips.Find(n => n.ShipName == newShipTypeContainer.Type) == null)
                    {
                        AllShips.Add(new ShipRecord()
                        {
                            ShipName = newShipTypeContainer.Type,
                            ShipNameCanonical = newShipTypeContainer.ShipTypeCanonical,
                            ShipNamespace = ns,
                            Instance = newShipTypeContainer
                        });

                        GetPilotsList(ns);
                    }
                }
            }
        }

        private static List<string> GetPilotsList(string shipName, Faction faction = Faction.None)
        {
            List<string> result = new List<string>();

            List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => String.Equals(t.Namespace, shipName, StringComparison.Ordinal))
                .ToList();

            foreach (var type in typelist)
            {
                if (type.MemberType == MemberTypes.NestedType) continue;

                GenericShip newShipContainer = (GenericShip)System.Activator.CreateInstance(type);
                if ((newShipContainer.PilotName != null) && (newShipContainer.IsAllowedForSquadBuilder()))
                {
                    if ((newShipContainer.faction == faction) || faction == Faction.None)
                    {
                        string pilotKey = newShipContainer.PilotName + " (" + newShipContainer.Cost + ")";

                        if (AllPilots.Find(n => n.PilotName == newShipContainer.PilotName && n.PilotShip.ShipName == newShipContainer.Type) == null)
                        {
                            AllPilots.Add(new PilotRecord()
                            {
                                PilotName = newShipContainer.PilotName,
                                //PilotNameWithCost = pilotKey,
                                PilotTypeName = type.ToString(),
                                PilotNameCanonical = newShipContainer.PilotNameCanonical,
                                PilotSkill = newShipContainer.PilotSkill,
                                PilotShip = AllShips.Find(n => n.ShipName == newShipContainer.Type),
                                Instance = newShipContainer
                            });
                        }

                        result.Add(pilotKey);
                    }
                }
            }

            return result;
        }

        public static void SetCurrentPlayerFaction(Faction faction)
        {
            CurrentSquadList.SquadFaction = faction;
        }

        public static void ShowInfo()
        {
            //Messages.ShowInfo("Ships loaded: " + AllShips.Count);
            //Messages.ShowInfo("Pilots loaded: " + AllPilots.Count);

            foreach (var ship in CurrentSquadList.GetShips())
            {
                Messages.ShowInfo(ship.Instance.PilotName);
            }
        }

        public static void ShowShipsFilteredByFaction()
        {
            ShowAvailableShips(CurrentSquadList.SquadFaction);
        }

        public static void ShowPilotsFilteredByShipAndFaction()
        {
            ShowAvailablePilots(CurrentSquadList.SquadFaction, CurrentShipToBrowsePilots);
        }

        public static void AddPilotToSquad(string pilotName, string shipName)
        {
            PilotRecord pilotRecord = AllPilots.Find(n => n.PilotName == pilotName && n.PilotShip.ShipName == shipName);
            CurrentSquadList.AddShip(pilotRecord);
        }

        public static void ShowShipsAndUpgrades()
        {
            GenerateShipWithUpgradesPanels();
        }

        public static void OpenSelectShip()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SelectShipPanel");
        }
    }
}
