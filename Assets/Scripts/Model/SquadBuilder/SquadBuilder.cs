﻿using System;
using System.Collections.Generic;
using System.Linq;
using Players;
using Ship;
using System.Reflection;
using UnityEngine;
using Upgrade;
using GameModes;
using UnityEngine.SceneManagement;
using Editions;
using Obstacles;
using System.IO;

namespace SquadBuilderNS
{
    public class SquadBuilderShip
    {
        public GenericShip Instance;
        public SquadList List;
        public SquadBuilder.ShipWithUpgradesPanel Panel;

        public SquadBuilderShip(GenericShip ship, SquadList list)
        {
            List = list;

            InitializeShip(ship);
        }

        private void InitializeShip(GenericShip ship)
        {
            Instance = ship;
            Instance.InitializePilotForSquadBuilder();
        }
    }

    public class SquadList
    {
        private List<SquadBuilderShip> Ships;
        public Faction SquadFaction;
        public Type PlayerType;
        public PlayerNo PlayerNo;
        public string Name;
        public JSONObject SavedConfiguration;
        public int Points;
        public List<GenericObstacle> ChosenObstacles;

        public SquadList(PlayerNo playerNo)
        {
            PlayerNo = playerNo;
            SetDefaultObstacles();
        }

        public void SetDefaultObstacles()
        {
            ChosenObstacles = new List<GenericObstacle>()
            {
                ObstaclesManager.GetPossibleObstacle("coreasteroid5"),
                ObstaclesManager.GetPossibleObstacle("core2asteroid5"),
                ObstaclesManager.GetPossibleObstacle("core2asteroid4")
            };
        }

        public SquadBuilderShip AddShip(GenericShip ship)
        {
            if (Ships == null) Ships = new List<SquadBuilderShip>();

            SquadBuilderShip newShip = new SquadBuilderShip(ship, this);
            Ships.Add(newShip);
            return newShip;
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

        public void ClearShips()
        {
            Ships = new List<SquadBuilderShip>();
            Points = 0;
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
        public Faction PilotFaction;
        public int PilotSkill;
        public ShipRecord PilotShip;
        public GenericShip Instance;
    }

    public class UpgradeRecord
    {
        public string UpgradeName;
        public string UpgradeNameCanonical;
        public string UpgradeTypeName;
        public UpgradeType UpgradeType;
        public GenericUpgrade Instance;
    }

    static partial class SquadBuilder
    {
        public static PlayerNo CurrentPlayer { get; private set; }
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
        public static GenericObstacle CurrentObstacle;

        public static void Initialize()
        {
            GenerateListOfShips();
            GenerateUpgradesList();
            SquadLists = new List<SquadList>()
            {
                new SquadList(PlayerNo.Player1),
                new SquadList(PlayerNo.Player2)
            };
        }

        public static void SetCurrentPlayer(PlayerNo playerNo)
        {
            CurrentPlayer = playerNo;
        }

        public static void ClearShipsOfPlayer(PlayerNo playerNo)
        {
            SquadList squadList = GetSquadList(playerNo);
            squadList.ClearShips();
            squadList.Name = GetDefaultNameForSquad(playerNo);
        }

        private static void GenerateListOfShips()
        {
            AllShips = new List<ShipRecord>();
            AllPilots = new List<PilotRecord>();

            string namespacePart = "Ship." + Edition.Current.NameShort + ".";

            IEnumerable<string> namespaceIEnum =
                from types in Assembly.GetExecutingAssembly().GetTypes()
                where types.Namespace != null
                where types.Namespace.StartsWith(namespacePart)
                select types.Namespace;

            List<string> namespaceList = new List<string>();
            foreach (var ns in namespaceIEnum)
            {
                if (!namespaceList.Contains(ns))
                {
                    namespaceList.Add(ns);
                    GenericShip newShipTypeContainer = (GenericShip)System.Activator.CreateInstance(System.Type.GetType(ns + "." + ns.Substring(namespacePart.Length)));
                    Edition.Current.AdaptShipToRules(newShipTypeContainer);

                    if (AllShips.Find(n => n.ShipName == newShipTypeContainer.ShipInfo.ShipName) == null)
                    {
                        AllShips.Add(new ShipRecord()
                        {
                            ShipName = newShipTypeContainer.ShipInfo.ShipName,
                            ShipNameCanonical = newShipTypeContainer.ShipTypeCanonical,
                            ShipNamespace = ns,
                            Instance = newShipTypeContainer
                        });

                        GetPilotsList(ns);
                    }
                }
            }

            // Check for unique pilot IDs

            if (!DebugManager.ReleaseVersion && !(Edition.Current is Editions.FirstEdition))
            {
                foreach (PilotRecord pilotRecord in AllPilots)
                {
                    int samePilotCount = 0;
                    foreach (PilotRecord pilotRecord2 in AllPilots)
                    {
                        if (pilotRecord.PilotNameCanonical == pilotRecord2.PilotNameCanonical)
                        {
                            samePilotCount++;
                            if (samePilotCount > 1)
                            {
                                Debug.LogError("Pilot ID " + pilotRecord.PilotNameCanonical + " has duplicate!");
                                break;
                            }
                        }
                    }
                }
            }

            //Messages.ShowInfo("Unique pilots: " + AllPilots.Count(n => n.Instance.IsUnique));
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
                Edition.Current.AdaptPilotToRules(newShipContainer);

                if ((newShipContainer.PilotInfo != null) && (newShipContainer.IsAllowedForSquadBuilder()))
                {
                    if ((newShipContainer.Faction == faction) || faction == Faction.None)
                    {
                        string pilotKey = newShipContainer.PilotInfo.PilotName + " (" + newShipContainer.PilotInfo.Cost + ")";

                        if (AllPilots.Find(n => n.PilotName == newShipContainer.PilotInfo.PilotName && n.PilotShip.ShipName == newShipContainer.ShipInfo.ShipName && n.PilotFaction == newShipContainer.Faction) == null)
                        {
                            AllPilots.Add(new PilotRecord()
                            {
                                PilotName = newShipContainer.PilotInfo.PilotName,
                                PilotTypeName = type.ToString(),
                                PilotNameCanonical = newShipContainer.PilotNameCanonical,
                                PilotSkill = newShipContainer.PilotInfo.Initiative,
                                PilotShip = AllShips.Find(n => n.ShipName == newShipContainer.ShipInfo.ShipName),
                                PilotFaction = newShipContainer.Faction,
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
                .Where(t => String.Equals(t.Namespace, "UpgradesList." + Edition.Current.NameShort, StringComparison.Ordinal))
                .ToList();

            foreach (var type in typelist)
            {
                if (type.MemberType == MemberTypes.NestedType) continue;

                GenericUpgrade newUpgradeContainer = (GenericUpgrade)System.Activator.CreateInstance(type);
                if ((newUpgradeContainer.UpgradeInfo.Name != null))
                {
                    if (AllUpgrades.Find(n => n.UpgradeNameCanonical == newUpgradeContainer.NameCanonical && n.UpgradeType == newUpgradeContainer.UpgradeInfo.UpgradeTypes.First()) == null)
                    {
                        Edition.Current.AdaptUpgradeToRules(newUpgradeContainer);

                        if (newUpgradeContainer.IsAllowedForSquadBuilder())
                        {
                            AllUpgrades.Add(new UpgradeRecord()
                            {
                                UpgradeName = newUpgradeContainer.UpgradeInfo.Name,
                                UpgradeNameCanonical = newUpgradeContainer.NameCanonical,
                                UpgradeTypeName = type.ToString(),
                                UpgradeType = newUpgradeContainer.UpgradeInfo.UpgradeTypes.First(),
                                Instance = newUpgradeContainer
                            });
                        }
                    }
                }
            }

            AllUpgrades = AllUpgrades.OrderBy(n => n.Instance.UpgradeInfo.Name).ToList();

            if (!DebugManager.ReleaseVersion && !(Edition.Current is Editions.FirstEdition))
            {
                foreach (UpgradeRecord upgradeRecord in AllUpgrades)
                {
                    int samePilotCount = 0;
                    foreach (UpgradeRecord upgradeRecord2 in AllUpgrades)
                    {
                        if (upgradeRecord.UpgradeNameCanonical == upgradeRecord2.UpgradeNameCanonical)
                        {
                            samePilotCount++;
                            if (samePilotCount > 1)
                            {
                                Debug.LogError("Upgrade ID " + upgradeRecord.UpgradeNameCanonical + " has duplicate!");
                                break;
                            }
                        }
                    }
                }
            }

            //Messages.ShowInfo("Upgrades: " + AllUpgrades.Count);
        }

        public static void SetCurrentPlayerFaction(Faction faction)
        {
            CurrentSquadList.SquadFaction = faction;
        }

        public static void ShowShipsFilteredByFaction()
        {
            ShowLoadingContentStub("Ship");
            ShowAvailableShips(CurrentSquadList.SquadFaction);
            UpdateSquadCostForShipsMenu(GetCurrentSquadCost());
        }

        public static void ShowPilotsFilteredByShipAndFaction()
        {
            ShowLoadingContentStub("Pilot");
            ShowAvailablePilots(CurrentSquadList.SquadFaction, CurrentShip);
            UpdateSquadCostForPilotsMenu(GetCurrentSquadCost());
        }

        private static SquadBuilderShip AddPilotToSquad(GenericShip ship, PlayerNo playerNo)
        {
            var squadBuilderShip = GetSquadList(playerNo).AddShip(ship);

            foreach (var upgradeType in ship.DefaultUpgrades)
            {
                var upgrade = (GenericUpgrade)Activator.CreateInstance(upgradeType);

                List<UpgradeSlot> slots = FindFreeSlots(squadBuilderShip, upgrade.UpgradeInfo.UpgradeTypes);
                if (slots.Count != 0)
                {
                    slots[0].PreInstallUpgrade(upgrade, ship);
                }
            }

            return squadBuilderShip;
        }

        private static void InstallUpgrade(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            slot.PreInstallUpgrade(upgrade, CurrentSquadBuilderShip.Instance);
        }

        private static bool InstallUpgrade(SquadBuilderShip ship, string upgradeNameCanonical, UpgradeType upgradeType)
        {
            string upgradeTypeName = AllUpgrades.Find(n => n.UpgradeNameCanonical == upgradeNameCanonical && n.UpgradeType == upgradeType).UpgradeTypeName;
            GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeTypeName));
            Edition.Current.AdaptUpgradeToRules(newUpgrade);
            if (newUpgrade is IVariableCost && Edition.Current is SecondEdition) (newUpgrade as IVariableCost).UpdateCost(ship.Instance);

            List<UpgradeSlot> slots = FindFreeSlots(ship, newUpgrade.UpgradeInfo.UpgradeTypes);
            if (slots.Count != 0)
            {
                slots[0].PreInstallUpgrade(newUpgrade, ship.Instance);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static List<UpgradeSlot> FindFreeSlots(SquadBuilderShip shipHolder, List<UpgradeType> upgradeTypes)
        {
            return shipHolder.Instance.UpgradeBar.GetFreeSlots(upgradeTypes);
        }

        public static void ShowShipsAndUpgrades()
        {
            UpdateSquadCostForSquadBuilder(GetCurrentSquadCost());
            GenerateShipWithUpgradesPanels();

            //Debug.Log(GetSquadInJson(CurrentPlayer).ToString());
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
            foreach (var shipHolder in GetSquadList(playerNo).GetShips())
            {
                result += GetShipCost(shipHolder);

            }
            return result;
        }

        private static int GetShipCost(SquadBuilderShip shipHolder)
        {
            int result = 0;

            result += shipHolder.Instance.PilotInfo.Cost;

            foreach (var upgradeSlot in shipHolder.Instance.UpgradeBar.GetUpgradeSlots())
            {
                if (!upgradeSlot.IsEmpty)
                {
                    result += ReduceUpgradeCost(upgradeSlot.InstalledUpgrade.UpgradeInfo.Cost, upgradeSlot.CostDecrease);
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

        private static void OpenShipInfo(GenericShip ship)
        {
            if (Edition.Current.IsSquadBuilderLocked)
            {
                Messages.ShowError("This part of squad builder is disabled");
                return;
            }

            CurrentSquadBuilderShip = CurrentSquadList.GetShips().Find(n => n.Instance == ship);
            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }

        public static void RemoveCurrentShip()
        {
            CurrentSquadList.RemoveShip(CurrentSquadBuilderShip);
        }

        private static void OpenSelectUpgradeMenu(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            CurrentUpgradeSlot = slot;
            MainMenu.CurrentMainMenu.ChangePanel("SelectUpgradePanel");
        }

        private static void RemoveInstalledUpgrade(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            slot.RemovePreInstallUpgrade();
            // check if upgrade is multi-slotted
            ShowPilotWithSlots();
        }

        public static void ShowUpgradesList()
        {
            ShowLoadingContentStub("Upgrade");
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
                case "Replay":
                    SetPlayerTypes(typeof(ReplayPlayer), typeof(ReplayPlayer));
                    break;
                default:
                    break;
            }
        }

        private static void SetPlayerTypes(Type playerOneType, Type playerTwoType)
        {
            GetSquadList(PlayerNo.Player1).PlayerType = playerOneType;
            GetSquadList(PlayerNo.Player2).PlayerType = playerTwoType;
        }

        public static void StartNetworkGame()
        {
            GameController.Initialize();
            ReplaysManager.Initialize(ReplaysMode.Write);

            Console.Write("Network game is started", LogTypes.GameCommands, true, "aqua");

            Network.StartNetworkGame();
        }

        public static void StartLocalGame()
        {
            GameMode.CurrentGameMode = new LocalGame();
            SwitchToBattleScene();
        }

        public static void SwitchToBattleScene()
        {
            Global.ToggelLoadingScreen(true);
            LoadBattleScene();
        }

        public static void SaveAutosaveSquadConfigurations()
        {
            for (int i = 0; i < 2; i++)
            {
                SaveSquadron(GetSquadList(Tools.IntToPlayer(i+1)), "Autosave (Player " + (i+1) + ")", delegate {
                    foreach (var squad in SquadLists)
                    {
                        squad.SavedConfiguration = GetSquadInJson(squad.PlayerNo);

                        JSONObject playerInfoJson = new JSONObject();
                        playerInfoJson.AddField("NickName", Options.NickName);
                        playerInfoJson.AddField("Title", Options.Title);
                        playerInfoJson.AddField("Avatar", Options.Avatar);
                        squad.SavedConfiguration.AddField("PlayerInfo", playerInfoJson);
                    }
                });
            }
        }

        public static void LoadBattleScene()
        {
            MainMenu.ShowAiInformation();
            SceneManager.LoadScene("Battle");
        }

        public static bool ValidateCurrentPlayersRoster()
        {
            return ValidatePlayerRoster(CurrentPlayer);
        }

        private static bool ValidatePlayerRoster(PlayerNo playerNo)
        {
            if (!ValidateMinShipsCount(playerNo)) return false;
            if (!ValidateMaxSameShipsCount(playerNo)) return false;
            if (!ValidateUniqueCards(playerNo)) return false;
            if (!ValidateSquadCost(playerNo)) return false;
            if (!ValidateFeLimitedCards(playerNo)) return false;
            if (!ValidateSolitaryCards(playerNo)) return false;
            if (!ValidateShipAiReady(playerNo)) return false;
            if (!ValidateUpgradePostChecks(playerNo)) return false;
            if (!ValidateSlotsRequirements(playerNo)) return false;

            return true;
        }

        private static bool ValidateMinShipsCount(PlayerNo playerNo)
        {
            bool result = true;
            if (GetSquadList(playerNo).GetShips().Count < Edition.Current.MinShipsCount)
            {
                result = false;
                Messages.ShowError("The minimum number of pilots required is: " + Edition.Current.MinShipsCount);
            }
            return result;
        }

        private static bool ValidateMaxSameShipsCount(PlayerNo playerNo)
        {
            bool result = true;

            Dictionary<string, int> shipTypesCount = new Dictionary<string, int>();

            foreach (GenericShip ship in GetSquadList(playerNo).GetShips().Select(n => n.Instance))
            {
                if (!shipTypesCount.ContainsKey(ship.ShipInfo.ShipName))
                {
                    shipTypesCount.Add(ship.ShipInfo.ShipName, 1);
                }
                else
                {
                    shipTypesCount[ship.ShipInfo.ShipName]++;
                    if (shipTypesCount[ship.ShipInfo.ShipName] > Edition.Current.MaxShipsCount)
                    {
                        Messages.ShowError("Too many ships of type \"" + ship.ShipInfo.ShipName + "\"");
                        Messages.ShowError("The maximum allowed number of same ships in squad is: " + Edition.Current.MaxShipsCount);
                        return false;
                    }
                }
            }

            if (shipTypesCount.Sum(n => n.Value) > 10)
            {
                Messages.ShowError("Current version of the game supports only 10 ships per player. Please, wait for support of epic mode.");
                result = false;
            }

            return result;
        }

        private static bool ValidateUniqueCards(PlayerNo playerNo)
        {
            bool result = true;

            Dictionary<string, int> uniqueCards = new Dictionary<string, int>();
            foreach (var shipConfig in GetSquadList(playerNo).GetShips())
            {
                if (shipConfig.Instance.PilotInfo.IsLimited)
                {
                    string cleanName = GetCleanName(shipConfig.Instance.PilotInfo.PilotName);
                    if (uniqueCards.ContainsKey(cleanName)) uniqueCards[cleanName]--; else uniqueCards.Add(cleanName, shipConfig.Instance.PilotInfo.Limited - 1);
                }

                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.IsLimited)
                    {
                        string cleanName = GetCleanName(upgrade.UpgradeInfo.Name);
                        if (uniqueCards.ContainsKey(cleanName)) uniqueCards[cleanName]--; else uniqueCards.Add(cleanName, upgrade.UpgradeInfo.Limited - 1);
                    }
                }
            }

            foreach (var uniqueCardInfo in uniqueCards)
            {
                if (uniqueCardInfo.Value < 0)
                {
                    Messages.ShowError("You have too many limited cards with the name " + uniqueCardInfo.Key);
                    result = false;
                }
            }

            return result;
        }

        private static string GetCleanName(string name)
        {
            if (name.Contains("(")) name = name.Substring(0, name.LastIndexOf("(") - 1);
            return name;
        }

        private static bool ValidateSquadCost(PlayerNo playerNo)
        {
            bool result = true;

            if (!DebugManager.DebugNoSquadPointsLimit)
            {
                if (GetSquadCost(playerNo) > Edition.Current.MaxPoints)
                {
                    Messages.ShowError("The cost of your squadron cannot be more than " + Edition.Current.MaxPoints);
                    result = false;
                }
            }

            return result;
        }

        private static bool ValidateFeLimitedCards(PlayerNo playerNo)
        {
            bool result = true;

            foreach (var shipConfig in GetSquadList(playerNo).GetShips())
            {
                List<string> feLmitedPerShipCards = new List<string>();

                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.FeIsLimitedPerShip)
                    {
                        if (!feLmitedPerShipCards.Contains(upgrade.UpgradeInfo.Name))
                        {
                            feLmitedPerShipCards.Add(upgrade.UpgradeInfo.Name);
                        }
                        else
                        {
                            Messages.ShowError("A ship cannot equip multiple copies of the same limited card: " + upgrade.UpgradeInfo.Name);
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private static bool ValidateSolitaryCards(PlayerNo playerNo)
        {
            bool result = true;

            int solitaryCards = 0;

            foreach (var shipConfig in GetSquadList(playerNo).GetShips())
            {
                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.IsSolitary)
                    {
                        if (solitaryCards == 0)
                        {
                            solitaryCards++;
                        }
                        else
                        {
                            result = false;
                            Messages.ShowError("Only one Solitary upgrade can be equipped");
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
            SquadList aiSquadlist = GetSquadList(playerNo);
            if (aiSquadlist.PlayerType == typeof(HotacAiPlayer))
            {
                foreach (var shipConfig in aiSquadlist.GetShips())
                {
                    if (shipConfig.Instance.HotacManeuverTable == null)
                    {
                        Messages.ShowError("AI for " + shipConfig.Instance.ShipInfo.ShipName + " is not ready, it can be controlled only by a human");
                        return false;
                    }
                }
            }

            return result;
        }

        private static bool ValidateUpgradePostChecks(PlayerNo playerNo)
        {
            bool result = true;

            SquadList squadList = GetSquadList(playerNo);

            foreach (var shipHolder in squadList.GetShips())
            {
                if (!shipHolder.Instance.IsAllowedForSquadBuilderPostCheck(squadList)) return false;

                foreach (var upgradeHolder in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (!upgradeHolder.IsAllowedForSquadBuilderPostCheck(squadList)) return false;
                }
            }

            return result;
        }

        private static bool ValidateSlotsRequirements(PlayerNo playerNo)
        {
            bool result = true;

            foreach (var shipHolder in GetSquadList(playerNo).GetShips())
            {
                foreach (var upgradeSlot in shipHolder.Instance.UpgradeBar.GetUpgradeSlots())
                {
                    if (!upgradeSlot.IsEmpty)
                    {
                        if (upgradeSlot.MustBeDifferent)
                        {
                            int countDuplicates = shipHolder.Instance.UpgradeBar.GetUpgradesAll().Count(n => n.UpgradeInfo.Name == upgradeSlot.InstalledUpgrade.UpgradeInfo.Name);
                            if (countDuplicates > 1)
                            {
                                Messages.ShowError("You cannot have more than one copy of " + upgradeSlot.InstalledUpgrade.UpgradeInfo.Name + " on one ship");
                                return false;
                            }
                        }
                        if (upgradeSlot.InstalledUpgrade.UpgradeInfo.Cost > upgradeSlot.MaxCost)
                        {
                            Messages.ShowError("The upgrade must costs less than " + upgradeSlot.MaxCost + " : " + upgradeSlot.InstalledUpgrade.UpgradeInfo.Name);
                            return false;
                        }
                        if (upgradeSlot.MustBeUnique && !upgradeSlot.InstalledUpgrade.UpgradeInfo.IsLimited)
                        {
                            Messages.ShowError("The upgrade must be unique : " + upgradeSlot.InstalledUpgrade.UpgradeInfo.Name);
                            return false;
                        }
                    }                
                }
            }

            return result;
        }

        // IMPORT / EXPORT

        public static void CreateSquadFromImportedJson(string name, string jsonString, PlayerNo playerNo, Action callback)
        {
            JSONObject squadJson = new JSONObject(jsonString);
            //LogImportedSquad(squadJson);

            SetPlayerSquadFromImportedJson(
                name,
                squadJson,
                playerNo,
                callback
            );
        }

        public static void SetPlayerSquadFromImportedJson(string name, JSONObject squadJson, PlayerNo playerNo, Action callBack)
        {
            ClearShipsOfPlayer(playerNo);

            try
            {
                SquadList squadList = GetSquadList(playerNo);

                if (squadJson.HasField("name"))
                {
                    squadList.Name = squadJson["name"].str;
                }

                string factionNameXws = squadJson["faction"].str;
                Faction faction = Edition.Current.XwsToFaction(factionNameXws);
                squadList.SquadFaction = faction;

                if (squadJson.HasField("points"))
                {
                    squadList.Points = (int)squadJson["points"].i;
                }

                if (squadJson.HasField("pilots"))
                {
                    JSONObject pilotJsons = squadJson["pilots"];
                    foreach (JSONObject pilotJson in pilotJsons.list)
                    {
                        string shipNameXws = pilotJson["ship"].str;
                        string shipNameGeneral = AllShips.Find(n => n.ShipNameCanonical == shipNameXws).ShipName;

                        string pilotNameXws = (Edition.Current is Editions.FirstEdition) ? pilotJson["name"].str : pilotJson["id"].str;
                        if (!AllPilots.Any(n => n.PilotNameCanonical == pilotNameXws))
                        {
                            Debug.Log("Cannot find pilot: " + pilotNameXws);
                            Console.Write("Cannot find pilot: " + pilotNameXws, LogTypes.Errors, true, "red");
                        }
                        string pilotNameGeneral = AllPilots.Find(n => n.PilotNameCanonical == pilotNameXws).PilotName;

                        PilotRecord pilotRecord = AllPilots.Find(n => n.PilotName == pilotNameGeneral && n.PilotShip.ShipName == shipNameGeneral && n.PilotFaction == faction);
                        GenericShip newShipInstance = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
                        Edition.Current.AdaptShipToRules(newShipInstance);
                        SquadBuilderShip newShip = AddPilotToSquad(newShipInstance, playerNo);

                        Dictionary<string, string> upgradesThatCannotBeInstalled = new Dictionary<string, string>();

                        if (pilotJson.HasField("upgrades"))
                        {
                            JSONObject upgradeJsons = pilotJson["upgrades"];
                            foreach (string upgradeType in upgradeJsons.keys)
                            {
                                JSONObject upgradeNames = upgradeJsons[upgradeType];
                                foreach (JSONObject upgradeRecord in upgradeNames.list)
                                {
                                    if (!AllUpgrades.Any(n => n.UpgradeNameCanonical == upgradeRecord.str))
                                    {
                                        Debug.Log("Cannot find upgrade: " + upgradeRecord.str);
                                        Console.Write("Cannot find upgrade: " + upgradeRecord.str, LogTypes.Errors, true, "red");
                                    }
                                    bool upgradeInstalledSucessfully = InstallUpgrade(newShip, upgradeRecord.str, Edition.Current.XwsToUpgradeType(upgradeType));
                                    if (!upgradeInstalledSucessfully) upgradesThatCannotBeInstalled.Add(upgradeRecord.str, upgradeType);
                                }
                            }

                            while (upgradeJsons.Count != 0)
                            {
                                Dictionary<string, string> upgradesThatCannotBeInstalledCopy = new Dictionary<string, string>(upgradesThatCannotBeInstalled);

                                bool wasSuccess = false;
                                foreach (var upgrade in upgradesThatCannotBeInstalledCopy)
                                {
                                    bool upgradeInstalledSucessfully = InstallUpgrade(newShip, upgrade.Key, Edition.Current.XwsToUpgradeType(upgrade.Value));
                                    if (upgradeInstalledSucessfully)
                                    {
                                        wasSuccess = true;
                                        upgradesThatCannotBeInstalled.Remove(upgrade.Key);
                                    }
                                }

                                if (!wasSuccess) break;
                            }
                        }

                        if (pilotJson.HasField("vendor"))
                        {
                            JSONObject vendorData = pilotJson["vendor"];
                            if (vendorData.HasField("Sandrem.FlyCasual"))
                            {
                                JSONObject myVendorData = vendorData["Sandrem.FlyCasual"];
                                if (myVendorData.HasField("skin"))
                                {
                                    newShip.Instance.ModelInfo.SkinName = myVendorData["skin"].str;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Messages.ShowError("The squad has no pilots");
                }

                if (squadJson.HasField("obstacles"))
                {
                    squadList.ChosenObstacles = new List<GenericObstacle>()
                    {
                        ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][0].str),
                        ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][1].str),
                        ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][2].str)
                    };
                }
                else
                {
                    squadList.SetDefaultObstacles();
                }

                callBack();
            }
            catch (Exception)
            {
                Messages.ShowError("An error occurred during the creation of squadron '" + name + "'");
                ClearShipsOfPlayer(playerNo);
                //throw;
            }
        }

        /*private static void LogImportedSquad(JSONObject squadJson)
        {
            if (squadJson.HasField("faction")) Debug.Log("Faction is " + squadJson["faction"]);
            if (squadJson.HasField("points")) Debug.Log("Points " + squadJson["points"]);

            if (squadJson.HasField("pilots"))
            {
                JSONObject pilotJsons = squadJson["pilots"];
                foreach (JSONObject pilotJson in pilotJsons.list)
                {
                    Debug.Log("PilotName " + pilotJson["name"]);
                    Debug.Log("Points " + pilotJson["points"]);
                    Debug.Log("ShipType " + pilotJson["ship"]);
                }
            }
        }

        public static void ExportSquadList(PlayerNo playerNo)
        {
            GameObject importExportPanel = GameObject.Find("UI/Panels").transform.Find("ImportExportPanel").gameObject;
            MainMenu.CurrentMainMenu.ChangePanel(importExportPanel);
            importExportPanel.transform.Find("InputField").GetComponent<InputField>().text = GetSquadInJson(playerNo).ToString();
        }*/

        public static JSONObject GetSquadInJson(PlayerNo playerNo)
        {
            JSONObject squadJson = new JSONObject();
            squadJson.AddField("name", GetSquadList(playerNo).Name);
            squadJson.AddField("faction", Edition.Current.FactionToXws(GetSquadList(playerNo).SquadFaction));
            squadJson.AddField("points", GetSquadCost(playerNo));
            squadJson.AddField("version", "0.3.0");

            List<SquadBuilderShip> playerShipConfigs = GetSquadList(playerNo).GetShips().ToList();
            JSONObject[] squadPilotsArrayJson = new JSONObject[playerShipConfigs.Count];
            for (int i = 0; i < squadPilotsArrayJson.Length; i++)
            {
                squadPilotsArrayJson[i] = GenerateSquadPilot(playerShipConfigs[i]);
            }
            JSONObject squadPilotsJson = new JSONObject(squadPilotsArrayJson);
            squadJson.AddField("pilots", squadPilotsJson);

            JSONObject squadObstalesArrayJson = new JSONObject(JSONObject.Type.ARRAY);
            for (int i = 0; i < GetSquadList(playerNo).ChosenObstacles.Count; i++)
            {
                squadObstalesArrayJson.Add(GetSquadList(playerNo).ChosenObstacles[i].ShortName);
            }

            squadJson.AddField("obstacles", squadObstalesArrayJson);
            squadJson.AddField("description", GetDescriptionOfSquadJson(squadJson));

            return squadJson;
        }

        private static string GetDescriptionOfSquadJson(JSONObject squadJson)
        {
            string result = "";

            if (squadJson.HasField("pilots"))
            {
                JSONObject pilotJsons = squadJson["pilots"];
                foreach (JSONObject pilotJson in pilotJsons.list)
                {
                    if (result != "") result += "\n";

                    string shipNameXws = pilotJson["ship"].str;
                    string shipNameGeneral = AllShips.Find(n => n.ShipNameCanonical == shipNameXws).ShipName;

                    string pilotNameXws = (Edition.Current is Editions.FirstEdition) ? pilotJson["name"].str : pilotJson["id"].str;
                    string pilotNameGeneral = AllPilots.Find(n => n.PilotNameCanonical == pilotNameXws).PilotName;

                    result += pilotNameGeneral;

                    if (AllPilots.Count(n => n.PilotName == pilotNameGeneral) > 1)
                    {
                        result += " (" + shipNameGeneral + ")";
                    }

                    JSONObject upgradeJsons = pilotJson["upgrades"];
                    foreach (string upgradeType in upgradeJsons.keys)
                    {
                        JSONObject upgradeNames = upgradeJsons[upgradeType];
                        foreach (JSONObject upgradeRecord in upgradeNames.list)
                        {
                            string upgradeName = AllUpgrades.Find(n => n.UpgradeNameCanonical == upgradeRecord.str).UpgradeName;
                            result += " + " + upgradeName;
                        }
                    }
                }
            }

            result = result.Replace("\"", "\\\"");

            return result;
        }

        private static JSONObject GenerateSquadPilot(SquadBuilderShip shipHolder)
        {
            JSONObject pilotJson = new JSONObject();
            pilotJson.AddField((Edition.Current is Editions.FirstEdition) ? "name" : "id", shipHolder.Instance.PilotNameCanonical);
            pilotJson.AddField("points", GetShipCost(shipHolder));
            pilotJson.AddField("ship", shipHolder.Instance.ShipTypeCanonical);

            Dictionary<string, JSONObject> upgradesDict = new Dictionary<string, JSONObject>();
            foreach (var installedUpgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
            {
                string slotName = Edition.Current.UpgradeTypeToXws(installedUpgrade.UpgradeInfo.UpgradeTypes[0]);
                if (!upgradesDict.ContainsKey(slotName))
                {
                    JSONObject upgrade = new JSONObject();
                    upgrade.Add(installedUpgrade.NameCanonical);
                    upgradesDict.Add(slotName, upgrade);
                }
                else
                {
                    upgradesDict[slotName].Add(installedUpgrade.NameCanonical);
                }
            }
            JSONObject upgradesDictJson = new JSONObject(upgradesDict);

            pilotJson.AddField("upgrades", upgradesDictJson);

            JSONObject vendorJson = new JSONObject();
            JSONObject skinJson = new JSONObject();
            skinJson.AddField("skin", GetSkinName(shipHolder));
            vendorJson.AddField("Sandrem.FlyCasual", skinJson);

            pilotJson.AddField("vendor", vendorJson);

            return pilotJson;
        }

        public static JSONObject GetSquadInJsonCompact(PlayerNo playerNo)
        {
            JSONObject squadJson = new JSONObject();

            List<SquadBuilderShip> playerShipConfigs = GetSquadList(playerNo).GetShips().ToList();
            JSONObject[] squadPilotsArrayJson = new JSONObject[playerShipConfigs.Count];
            for (int i = 0; i < squadPilotsArrayJson.Length; i++)
            {
                squadPilotsArrayJson[i] = GenerateSquadPilotCompact(playerShipConfigs[i]);
            }
            JSONObject squadPilotsJson = new JSONObject(squadPilotsArrayJson);
            squadJson.AddField("pilots", squadPilotsJson);

            return squadJson;
        }

        private static JSONObject GenerateSquadPilotCompact(SquadBuilderShip shipHolder)
        {
            JSONObject pilotJson = new JSONObject();
            pilotJson.AddField("n", shipHolder.Instance.PilotNameCanonical);

            string upgradesList = "";
            foreach (var installedUpgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
            {
                upgradesList += installedUpgrade.NameCanonical + " ";
            }
            JSONObject upgradesDictJson = new JSONObject(upgradesList);

            pilotJson.AddField("u", upgradesDictJson);

            return pilotJson;
        }

        public static bool IsNetworkGame
        {
            get { return GetSquadList(PlayerNo.Player2).PlayerType == typeof(NetworkOpponentPlayer); }
        }

        public static bool IsVsAiGame
        {
            get { return GetSquadList(PlayerNo.Player2).PlayerType.IsSubclassOf(typeof(GenericAiPlayer)); }
        }

        public static void SwitchPlayers()
        {
            SquadList player1SquadList = GetSquadList(PlayerNo.Player1);
            SquadList player2SquadList = GetSquadList(PlayerNo.Player2);

            player1SquadList.PlayerNo = PlayerNo.Player2;
            player2SquadList.PlayerNo = PlayerNo.Player1;
        }

        public static SquadList GetSquadList(PlayerNo playerNo)
        {
            return SquadLists.Find(n => n.PlayerNo == playerNo);
        }

        private static string GetSkinName(SquadBuilderShip shipHolder)
        {
            return shipHolder.Instance.ModelInfo.SkinName;
        }

        public static void BrowseSavedSquads()
        {
            string filename = "";
            // TEMPORARY
            GetRandomAiSquad(out filename);

            List<JSONObject> sortedSavedSquadsJsons = GetSavedSquadsJsons();
            foreach (string autosaveName in new List<string>() { "Autosave", "Autosave (Player 2)", "Autosave (Player 1)" })
            {
                SetAutosavesOnTop(sortedSavedSquadsJsons, autosaveName);
            }

            ShowListOfSavedSquadrons(sortedSavedSquadsJsons);
        }

        private static void SetAutosavesOnTop(List<JSONObject> jsonList, string autosaveName)
        {
            try
            {
                JSONObject autosaveJson = jsonList.Find(n => n["name"].str == autosaveName);
                if (autosaveJson != null)
                {
                    jsonList.Remove(autosaveJson);
                    jsonList.Insert(0, autosaveJson);
                }
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        private static void DeleteSavedSquadAndRefresh(string fileName)
        {
            if (Edition.Current.IsSquadBuilderLocked)
            {
                Messages.ShowError("This part of squad builder is disabled");
                return;
            }

            DeleteSavedSquadFile(fileName);
            BrowseSavedSquads();
        }

        private static void LoadSavedSquadAndReturn(string fileName)
        {
            JSONObject squadJson = GetSavedSquadJson(fileName);
            SetPlayerSquadFromImportedJson(fileName, squadJson, CurrentPlayer, ReturnToSquadBuilder);
        }

        public static void SetDefaultPlayerNames()
        {
            GetSquadList(PlayerNo.Player1).Name = GetDefaultNameForSquad(PlayerNo.Player1);
            GetSquadList(PlayerNo.Player2).Name = GetDefaultNameForSquad(PlayerNo.Player2);
        }

        private static string GetDefaultNameForSquad(PlayerNo playerNo)
        {
            string result = "Unknown Squadron";

            Type playerOneType = GetSquadList(PlayerNo.Player1).PlayerType;
            Type playerTwoType = GetSquadList(PlayerNo.Player2).PlayerType;

            if (playerOneType == typeof(HumanPlayer) && playerTwoType == typeof(NetworkOpponentPlayer))
            {
                if (playerNo == PlayerNo.Player1) return "My Squadron";
            }
            else if (playerOneType == typeof(HumanPlayer) && playerTwoType == typeof(HumanPlayer))
            {
                if (playerNo == PlayerNo.Player1) return "Squadron of Player 1"; else return "Squadron of Player 2";
            }
            else if (playerOneType == typeof(HumanPlayer) && playerTwoType.IsSubclassOf(typeof(GenericAiPlayer)))
            {
                if (playerNo == PlayerNo.Player1) return "My Squadron"; else return "Squadron of AI";
            }
            else if (playerOneType.IsSubclassOf(typeof(GenericAiPlayer)) && playerTwoType.IsSubclassOf(typeof(GenericAiPlayer)))
            {
                if (playerNo == PlayerNo.Player1) return "Squadron of AI 1"; else return "Squadron of AI 2";
            }

            return result;
        }

        private static void SetSkinForShip(SquadBuilderShip ship, string skinName)
        {
            ship.Instance.ModelInfo.SkinName = skinName;
        }

        public static void ReGenerateSquads(Action callback)
        {
            ReGenerateSquadOfPlayer(PlayerNo.Player1, delegate { ReGenerateSquadOfPlayer(PlayerNo.Player2, callback); });
        }

        private static void ReGenerateSquadOfPlayer(PlayerNo playerNo, Action callback)
        {
            JSONObject playerJson = GetSquadList(playerNo).SavedConfiguration;
            SetPlayerSquadFromImportedJson("", playerJson, playerNo, callback);
        }

        public static void SetAiType(string aiName)
        {
            SquadList currentSquadList = GetSquadList(CurrentPlayer);

            currentSquadList.PlayerType = typeof(AggressorAiPlayer);

            // ALTERNATIVE AI IS DISABLED

            /*switch (aiName)
            {
                case "AI: Aggressor":
                    currentSquadList.PlayerType = typeof(AggressorAiPlayer);
                    break;
                case "AI: HotAC":
                    currentSquadList.PlayerType = typeof(HotacAiPlayer);
                    break;
                default:
                    break;
            }

            GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadBuilderTop").transform.Find("AIButton").GetComponentInChildren<UnityEngine.UI.Text>().text = aiName;

            Options.AiType = aiName;
            Options.ChangeParameterValue("AiType", aiName); */
        }

        public static void ToggleAiType()
        {
            SquadList currentSquadList = GetSquadList(CurrentPlayer);
            string aiName = (currentSquadList.PlayerType == typeof(HotacAiPlayer)) ? "AI: Aggressor" : "AI: HotAC";

            SetAiType(aiName);
        }

        public static void ClearUpgradesOfCurrentShip()
        {
            ClearUpgradesOfCurrentShipRecursive();
        }

        private static void ClearUpgradesOfCurrentShipRecursive()
        {
            UpgradeSlot slot = CurrentSquadBuilderShip.Instance.UpgradeBar.GetUpgradeSlots().FirstOrDefault(s => !s.IsEmpty);
            if (slot != null)
            {
                slot.RemovePreInstallUpgrade();
                ClearUpgradesOfCurrentShipRecursive();
            }
            else
            {
                ShowPilotWithSlots();
            }
        }

        public static void CopyCurrentShip()
        {
            GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(CurrentSquadBuilderShip.Instance.GetType().ToString()));
            Edition.Current.AdaptShipToRules(newShip);
            Edition.Current.AdaptPilotToRules(newShip);

            SquadBuilderShip squadBuilderShip = CurrentSquadList.AddShip(newShip);
            List<GenericUpgrade> copyUpgradesList = new List<GenericUpgrade>(CurrentSquadBuilderShip.Instance.UpgradeBar.GetUpgradesAll());
            CopyUpgradesRecursive(squadBuilderShip, copyUpgradesList);
        }

        private static void CopyUpgradesRecursive(SquadBuilderShip targetShip, List<GenericUpgrade> upgradeList)
        {
            if (upgradeList.Count > 0)
            {
                InstallUpgrade(targetShip, upgradeList.First().NameCanonical, upgradeList.First().UpgradeInfo.UpgradeTypes.First());
                upgradeList.Remove(upgradeList.First());

                CopyUpgradesRecursive(targetShip, upgradeList);
            }
            else
            {
                UpdateSquadCostForPilotMenu(GetCurrentSquadCost());
            }
        }

        public static void SetDefaultObstacles()
        {
            CurrentSquadList.SetDefaultObstacles();
            ShowChosenObstaclesPanel();
        }
    }
}
