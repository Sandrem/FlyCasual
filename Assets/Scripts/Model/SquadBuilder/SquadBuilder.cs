using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Players;
using Ship;
using System.Reflection;
using UnityEngine;
using Upgrade;
using GameModes;
using UnityEngine.SceneManagement;

namespace SquadBuilderNS
{
    public class SquadBuilderShip
    {
        public List<SquadBuilderUpgrade> InstalledUpgrades;
        public GenericShip Instance;
        public SquadList List;
        public ShipWithUpgradesPanel Panel;

        public SquadBuilderShip(PilotRecord pilotRecord, SquadList list)
        {
            List = list;

            InitializeShip(pilotRecord);
        }

        private void InitializeShip(PilotRecord pilotRecord)
        {
            Instance = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
            Instance.InitializePilotForSquadBuilder();
        }
    }

    public class SquadBuilderUpgrade
    {

    }

    public class SquadList
    {
        private List<SquadBuilderShip> Ships;
        public Faction SquadFaction;
        public Type PlayerType;
        public PlayerNo PlayerNo;

        public SquadList(PlayerNo playerNo)
        {
            PlayerNo = playerNo;
        }

        public void AddShip(PilotRecord pilotRecord)
        {
            if (Ships == null) Ships = new List<SquadBuilderShip>();
            Ships.Add(new SquadBuilderShip(pilotRecord, this));
        }

        public void RemoveShip(SquadBuilderShip ship)
        {
            Ships.Remove(ship);
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

    public class UpgradeRecord
    {
        public string UpgradeName;
        public string UpgradeNameCanonical;
        public string UpgradeTypeName;
        public GenericUpgrade Instance;
    }

    static partial class SquadBuilder
    {
        public static PlayerNo CurrentPlayer;
        public static List<SquadList> SquadLists;
        public static SquadList CurrentSquadList
        {
            get { return SquadLists.Find(n => n.PlayerNo == CurrentPlayer); }
        }

        public static List<ShipRecord> AllShips;
        public static List<PilotRecord> AllPilots;
        public static List<UpgradeRecord> AllUpgrades;

        public static string CurrentShip;
        public static SquadBuilderShip CurrentSquadBuilderShip;
        public static UpgradeSlot CurrentUpgradeSlot;

        public static void Initialize()
        {
            CurrentPlayer = PlayerNo.Player1;

            SquadLists = new List<SquadList>()
            {
                new SquadList(PlayerNo.Player1),
                new SquadList(PlayerNo.Player2)
            };
            GenerateListOfShips();
            GenerateUpgradesList();
        }

        public static void NextPlayer()
        {
            CurrentPlayer = PlayerNo.Player2;
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

        private static void GenerateUpgradesList()
        {
            AllUpgrades = new List<UpgradeRecord>();

            List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => String.Equals(t.Namespace, "UpgradesList", StringComparison.Ordinal))
                .ToList();

            foreach (var type in typelist)
            {
                if (type.MemberType == MemberTypes.NestedType) continue;

                GenericUpgrade newUpgradeContainer = (GenericUpgrade)System.Activator.CreateInstance(type);
                if ((newUpgradeContainer.Name != null) && (newUpgradeContainer.IsAllowedForSquadBuilder()))
                {
                    if (AllUpgrades.Find(n => n.UpgradeName == newUpgradeContainer.Name) == null)
                    {
                        AllUpgrades.Add(new UpgradeRecord()
                        {
                            UpgradeName = newUpgradeContainer.Name,
                            UpgradeNameCanonical = newUpgradeContainer.NameCanonical,
                            UpgradeTypeName = type.ToString(),
                            Instance = newUpgradeContainer
                        });
                    }
                }
            }

            AllUpgrades = AllUpgrades.OrderBy(n => n.Instance.Name).OrderBy(m => m.Instance.Cost).ToList();

            //Messages.ShowInfo("Upgrades loaded: " + AllUpgrades.Count);
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
            ShowAvailablePilots(CurrentSquadList.SquadFaction, CurrentShip);
        }

        public static void AddPilotToSquad(string pilotName, string shipName)
        {
            PilotRecord pilotRecord = AllPilots.Find(n => n.PilotName == pilotName && n.PilotShip.ShipName == shipName);
            CurrentSquadList.AddShip(pilotRecord);
        }

        public static void ShowShipsAndUpgrades()
        {
            UpdateSquadCostForSquadBuilder(GetSquadCost());
            GenerateShipWithUpgradesPanels();
        }

        public static void UpdateNextButton()
        {
            ShowNextButtonFor(CurrentPlayer);
        }

        public static void ShowPilotWithSlots()
        {
            UpdateSquadCostForPilotMenu(GetSquadCost());
            GenerateShipWithSlotsPanels();
        }

        public static void OpenSelectShip()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SelectShipPanel");
        }

        private static int GetSquadCost()
        {
            int result = 0;
            foreach (var shipHolder in CurrentSquadList.GetShips())
            {
                result += shipHolder.Instance.Cost;
                result += shipHolder.Instance.UpgradeBar.GetInstalledUpgrades().Sum(n => n.Cost);
            }
            return result;
        }

        private static void OpenShipInfo(SquadBuilderShip ship, string pilotName, string shipName)
        {
            CurrentSquadBuilderShip = ship;
            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }

        public static void RemoveCurrentShip()
        {
            CurrentSquadList.RemoveShip(CurrentSquadBuilderShip);
        }

        private static void OpenSelectUpgradeMenu(UpgradeSlot slot, string upgradeName)
        {
            CurrentUpgradeSlot = slot;
            MainMenu.CurrentMainMenu.ChangePanel("SelectUpgradePanel");
        }

        private static void RemoveInstalledUpgrade(UpgradeSlot slot, string upgradeName)
        {
            slot.RemovePreInstallUpgrade();
            ShowPilotWithSlots();
        }

        public static void ShowUpgradesList()
        {
            UpdateSquadCostForUpgradesMenu(GetSquadCost());
            ShowAvailableUpgrades(CurrentUpgradeSlot);
        }

        public static void SetPlayers(string modeName)
        {
            switch (modeName)
            {
                case "vsAI":
                    SetPlayerTypes(typeof(HumanPlayer), typeof(HotacAiPlayer));
                    break;
                case "Internet":
                    SetPlayerTypes(typeof(HumanPlayer), typeof(NetworkOpponentPlayer));
                    break;
                case "HotSeat":
                    SetPlayerTypes(typeof(HumanPlayer), typeof(HumanPlayer));
                    break;
                case "AIvsAI":
                    SetPlayerTypes(typeof(HotacAiPlayer), typeof(HotacAiPlayer));
                    break;
                default:
                    break;
            }
        }

        private static void SetPlayerTypes(Type playerOneType, Type playerTwoType)
        {
            SquadLists.Find(n => n.PlayerNo == PlayerNo.Player1).PlayerType = playerOneType;
            SquadLists.Find(n => n.PlayerNo == PlayerNo.Player2).PlayerType = playerTwoType;
        }

        public static void StartLocalGame()
        {
            GameMode.CurrentGameMode = new LocalGame();

            //if (ValidatePlayersRosters())
            {
                ShowOpponentSquad();
                LoadBattleScene();
            }
        }

        private static void LoadBattleScene()
        {
            //TestRandom();
            SceneManager.LoadScene("Battle");
        }
    }
}
