using System;
using System.Collections.Generic;
using System.Linq;
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
            UpdateSquadCostForSquadBuilder(GetCurrentSquadCost());
            GenerateShipWithUpgradesPanels();
        }

        public static void UpdateNextButton()
        {
            ShowNextButtonFor(CurrentPlayer);
        }

        public static void ShowPilotWithSlots()
        {
            UpdateSquadCostForPilotMenu(GetCurrentSquadCost());
            GenerateShipWithSlotsPanels();
        }

        public static void OpenSelectShip()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SelectShipPanel");
        }

        private static int GetCurrentSquadCost()
        {
            return GetSquadCost(CurrentPlayer);
        }

        private static int GetSquadCost(PlayerNo playerNo)
        {
            int result = 0;
            foreach (var shipHolder in SquadLists.Find(n => n.PlayerNo == playerNo).GetShips())
            {
                result += shipHolder.Instance.Cost;

                foreach (var upgradeSlot in shipHolder.Instance.UpgradeBar.GetUpgradeSlots())
                {
                    if (!upgradeSlot.IsEmpty)
                    {
                        result += ReduceUpgradeCost(upgradeSlot.InstalledUpgrade.Cost, upgradeSlot.CostDecrease);
                    }
                }
            }
            return result;
        }

        private static int ReduceUpgradeCost(int cost, int decrease)
        {
            if (cost >= 0)
            {
                cost = Math.Max(cost - decrease, 0);
            }

            return cost;
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
            UpdateSquadCostForUpgradesMenu(GetCurrentSquadCost());
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

            if (ValidateCurrentPlayersRoster())
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

        public static bool ValidateCurrentPlayersRoster()
        {
            return ValidatePlayerRoster(CurrentPlayer);
        }

        private static bool ValidatePlayerRoster(PlayerNo playerNo)
        {
            if (!ValidateUniqueCards(playerNo)) return false;
            if (!ValidateSquadCost(playerNo)) return false;
            if (!ValidateLimitedCards(playerNo)) return false;
            if (!ValidateShipAiReady(playerNo)) return false;
            if (!ValidateUpgradePostChecks(playerNo)) return false;
            if (!ValidateDifferentUpgradesInAdditionalSlots(playerNo)) return false;

            return true;
        }

        private static bool ValidateUniqueCards(PlayerNo playerNo)
        {
            bool result = true;

            List<string> uniqueCards = new List<string>();
            foreach (var shipConfig in SquadLists.Find(n => n.PlayerNo == playerNo).GetShips())
            {
                if (shipConfig.Instance.IsUnique)
                {
                    if (CheckDuplicate(uniqueCards, shipConfig.Instance.PilotName)) return false;
                }

                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetInstalledUpgrades())
                {
                    if (upgrade.isUnique)
                    {
                        if (CheckDuplicate(uniqueCards, upgrade.Name)) return false;
                    }
                }
            }
            return result;
        }

        private static bool CheckDuplicate(List<string> uniqueCards, string cardName)
        {
            if (cardName.Contains("(")) cardName = cardName.Substring(0, cardName.LastIndexOf("(") - 1);
            if (uniqueCards.Contains(cardName))
            {
                Messages.ShowError("Only one card with unique name " + cardName + " can be present");
                return true;
            }
            else
            {
                uniqueCards.Add(cardName);
                return false;
            }
        }

        private static bool ValidateSquadCost(PlayerNo playerNo)
        {
            bool result = true;

            if (!DebugManager.DebugNoSquadPointsLimit)
            {
                if (GetSquadCost(playerNo) > 100)
                {
                    Messages.ShowError("Cost of squadron cannot be more than 100");
                    result = false;
                }
            }

            return result;
        }

        private static bool ValidateLimitedCards(PlayerNo playerNo)
        {
            bool result = true;

            foreach (var shipConfig in SquadLists.Find(n => n.PlayerNo == playerNo).GetShips())
            {
                List<string> limitedCards = new List<string>();

                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetInstalledUpgrades())
                {
                    if (upgrade.isLimited)
                    {
                        if (!limitedCards.Contains(upgrade.Name))
                        {
                            limitedCards.Add(upgrade.Name);
                        }
                        else
                        {
                            Messages.ShowError("A ship cannot equip multiple copies of the same limited card: " + upgrade.Name);
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private static bool ValidateShipAiReady(PlayerNo playerNo)
        {
            bool result = true;
            SquadList aiSquadlist = SquadLists.Find(n => n.PlayerNo == playerNo);
            if (aiSquadlist.PlayerType == typeof(HotacAiPlayer))
            {
                foreach (var shipConfig in aiSquadlist.GetShips())
                {
                    if (shipConfig.Instance.HotacManeuverTable == null)
                    {
                        Messages.ShowError("AI for " + shipConfig.Instance.Type + " is not ready. It can be controlled only by human.");
                        return false;
                    }
                }
            }

            return result;
        }

        private static bool ValidateUpgradePostChecks(PlayerNo playerNo)
        {
            bool result = true;

            SquadList squadList = SquadLists.Find(n => n.PlayerNo == playerNo);

            foreach (var shipHolder in squadList.GetShips())
            {
                foreach (var upgradeHolder in shipHolder.Instance.UpgradeBar.GetInstalledUpgrades())
                {
                    if (!upgradeHolder.IsAllowedForSquadBuilderPostCheck(squadList)) return false;
                }
            }

            return result;
        }

        private static bool ValidateDifferentUpgradesInAdditionalSlots(PlayerNo playerNo)
        {
            bool result = true;

            foreach (var shipHolder in SquadLists.Find(n => n.PlayerNo == playerNo).GetShips())
            {
                foreach (var upgradeSlot in shipHolder.Instance.UpgradeBar.GetUpgradeSlots())
                {
                    if (upgradeSlot.MustBeDifferent && !upgradeSlot.IsEmpty)
                    {
                        int countDuplicates = shipHolder.Instance.UpgradeBar.GetInstalledUpgrades().Count(n => n.Name == upgradeSlot.InstalledUpgrade.Name);
                        if (countDuplicates > 1)
                        {
                            Messages.ShowError("Upgrades must be different: " + upgradeSlot.InstalledUpgrade.Name);
                            return false;
                        }
                    }
                }
            }

            return result;
        }
    }
}
