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
        public SquadBuilder.ShipWithUpgradesPanel Panel;

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

        public void ClearShips()
        {
            Ships = new List<SquadBuilderShip>();
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

        public static void Initialize()
        {
            SquadLists = new List<SquadList>()
            {
                new SquadList(PlayerNo.Player1),
                new SquadList(PlayerNo.Player2)
            };
            GenerateListOfShips();
            GenerateUpgradesList();
        }

        public static void SetCurrentPlayer(PlayerNo playerNo)
        {
            CurrentPlayer = playerNo;
        }

        public static void ClearShipsOfCurrentPlayer()
        {
            CurrentSquadList.ClearShips();
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

        public static void StartNetworkGame()
        {
            //Network.Test();
            //Network.CallBacksTest();

            Network.StartNetworkGame();
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
            if (RosterIsEmpty(playerNo)) return false;
            if (!ValidateUniqueCards(playerNo)) return false;
            if (!ValidateSquadCost(playerNo)) return false;
            if (!ValidateLimitedCards(playerNo)) return false;
            if (!ValidateShipAiReady(playerNo)) return false;
            if (!ValidateUpgradePostChecks(playerNo)) return false;
            if (!ValidateDifferentUpgradesInAdditionalSlots(playerNo)) return false;

            return true;
        }

        private static bool RosterIsEmpty(PlayerNo playerNo)
        {
            bool result = false;
            if (SquadLists.Find(n => n.PlayerNo == playerNo).GetShips().Count == 0)
            {
                result = true;
                Messages.ShowError("At least one pilot must be present");
            }
            return result;
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

        // OLD

        /*private static List<string> GetSkins(GenericShip ship)
        {
            List<string> result = new List<string>();

            UnityEngine.Object[] textures = Resources.LoadAll("ShipSkins/" + ship.FixTypeName(ship.Type) + "/");
            foreach (var texture in textures)
            {
                result.Add(texture.name);
            }

            return result;
        }*/

        // IMPORT / EXPORT

        /*public static void ImportSquadList()
        {
            GameObject importExportPanel = GameObject.Find("UI/Panels").transform.Find("ImportExportPanel").gameObject;
            importExportPanel.transform.Find("InputField").GetComponent<InputField>().text = "";
            MainMenu.CurrentMainMenu.ChangePanel(importExportPanel);
        }

        public static void CreateSquadFromImportedjson(string jsonString, PlayerNo playerNo)
        {
            JSONObject squadJson = new JSONObject(jsonString);
            //LogImportedSquad(squadJson);

            SetPlayerSquadFromImportedJson(squadJson, playerNo, ShowRoster);
        }

        public static void RemoveAllShips()
        {
            RemoveAllShipsByPlayer(PlayerNo.Player1);
            RemoveAllShipsByPlayer(PlayerNo.Player2);
        }

        public static void RemoveAllShipsByPlayer(PlayerNo playerNo)
        {
            List<SquadBuilderShip> shipsList = SquadBuilderRoster.GetShipsByPlayer(playerNo);
            foreach (var ship in shipsList)
            {
                RemoveShip(playerNo, ship.Panel);
            }
        }

        private static void ShowRoster()
        {
            GameObject rosterBuilderPanel = GameObject.Find("UI/Panels").transform.Find("RosterBuilderPanel").gameObject;
            MainMenu.CurrentMainMenu.ChangePanel(rosterBuilderPanel);
        }

        public static void SwapRosters(Action callBack)
        {
            JSONObject p1squad = GetSquadInJson(PlayerNo.Player1);

            SetPlayerSquadFromImportedJson(p1squad, PlayerNo.Player2, callBack);
        }

        public static void SetPlayerSquadFromImportedJson(JSONObject squadJson, PlayerNo playerNo, Action callBack)
        {
            string factionNameXws = squadJson["faction"].str;
            string factionName = XWSToFactionName(factionNameXws);
            Dropdown factionDropdown = GetPlayerPanel(playerNo).Find("GroupFaction/Dropdown").GetComponent<Dropdown>();
            factionDropdown.value = factionDropdown.options.IndexOf(factionDropdown.options.Find(n => n.text == factionName));

            CheckPlayerFactonChange(playerNo);

            RemoveAllShipsByPlayer(playerNo);

            if (squadJson.HasField("pilots"))
            {
                JSONObject pilotJsons = squadJson["pilots"];
                foreach (JSONObject pilotJson in pilotJsons.list)
                {
                    SquadBuilderShip newShip = AddShip(playerNo);

                    string shipNameXws = pilotJson["ship"].str;
                    string shipNameGeneral = AllShips.Find(n => n.ShipNameCanonical == shipNameXws).ShipName;
                    Dropdown shipDropdown = newShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>();
                    shipDropdown.value = shipDropdown.options.IndexOf(shipDropdown.options.Find(n => n.text == shipNameGeneral));

                    string pilotNameXws = pilotJson["name"].str;
                    string pilotNameGeneral = AllPilots.Find(n => n.PilotNameCanonical == pilotNameXws).PilotNameWithCost;
                    Dropdown pilotDropdown = newShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>();
                    pilotDropdown.value = pilotDropdown.options.IndexOf(pilotDropdown.options.Find(n => n.text == pilotNameGeneral));

                    JSONObject upgradeJsons = pilotJson["upgrades"];
                    foreach (string upgradeType in upgradeJsons.keys)
                    {
                        JSONObject upgradeNames = upgradeJsons[upgradeType];
                        foreach (JSONObject upgradeName in upgradeNames.list)
                        {
                            InstallUpgradeIntoShipPanel(
                                newShip,
                                XwsToUpgradeType(upgradeType),
                                AllUpgrades.Find(n => n.UpgradeNameCanonical == upgradeName.str).UpgradeNameWithCost
                            );
                        }
                    }
                }
            }
            else
            {
                Messages.ShowError("No pilots");
            }

            callBack();
        }

        private static void InstallUpgradeIntoShipPanel(SquadBuilderShip ship, UpgradeType upgradeType, string upgradeNameWithCost)
        {
            Transform upgradesGroup = ship.Panel.transform.Find("GroupUpgrades");
            foreach (Transform upgradeLine in upgradesGroup)
            {
                if (upgradeLine.name == "Upgrade" + upgradeType.ToString() + "Line")
                {
                    Dropdown upgradeDropdown = upgradeLine.GetComponent<Dropdown>();
                    if (upgradeDropdown.value == 0)
                    {
                        upgradeDropdown.value = upgradeDropdown.options.IndexOf(upgradeDropdown.options.Find(n => n.text == upgradeNameWithCost));
                        break;
                    }
                }
            }
        }

        private static void LogImportedSquad(JSONObject squadJson)
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
        }

        public static JSONObject GetSquadInJson(PlayerNo playerNo)
        {
            JSONObject squadJson = new JSONObject();
            //squadJson.AddField("name", "New Squad");
            squadJson.AddField("faction", FactionToXWS(GetPlayerFaction(playerNo)));
            squadJson.AddField("points", GetPlayerShipsCostCalculated(playerNo));
            squadJson.AddField("version", "0.3.0");
            //squadJson.AddField("description", "No descripton");

            List<SquadBuilderShip> playerShipConfigs = SquadBuilderRoster.GetShips().Where(n => n.Player == playerNo).ToList();
            JSONObject[] squadPilotsArrayJson = new JSONObject[playerShipConfigs.Count];
            for (int i = 0; i < squadPilotsArrayJson.Length; i++)
            {
                squadPilotsArrayJson[i] = GenerateSquadPilot(playerShipConfigs[i]);
            }
            JSONObject squadPilotsJson = new JSONObject(squadPilotsArrayJson);
            squadJson.AddField("pilots", squadPilotsJson);

            return squadJson;
        }

        private static JSONObject GenerateSquadPilot(SquadBuilderShip shipHolder)
        {
            JSONObject pilotJson = new JSONObject();
            pilotJson.AddField("name", shipHolder.Ship.PilotNameCanonical);
            pilotJson.AddField("points", GetShipCostCalculated(shipHolder));
            pilotJson.AddField("ship", shipHolder.Ship.ShipTypeCanonical);

            Dictionary<string, JSONObject> upgradesDict = new Dictionary<string, JSONObject>();
            foreach (var slotHolder in shipHolder.GetUpgrades())
            {
                if (slotHolder.Slot.InstalledUpgrade != null)
                {
                    string slotName = UpgradeTypeToXWS(slotHolder.Slot.Type);
                    if (!upgradesDict.ContainsKey(slotName))
                    {
                        JSONObject upgrade = new JSONObject();
                        upgrade.Add(slotHolder.Slot.InstalledUpgrade.NameCanonical);
                        upgradesDict.Add(slotName, upgrade);
                    }
                    else
                    {
                        upgradesDict[slotName].Add(slotHolder.Slot.InstalledUpgrade.NameCanonical);
                    }
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

        private static string FactionToXWS(Faction faction)
        {
            string result = "";

            switch (faction)
            {
                case Faction.Rebel:
                    result = "rebel";
                    break;
                case Faction.Imperial:
                    result = "imperial";
                    break;
                case Faction.Scum:
                    result = "scum";
                    break;
                default:
                    break;
            }

            return result;
        }

        private static string XWSToFactionName(string factionXWS)
        {
            string result = "";

            switch (factionXWS)
            {
                case "rebel":
                    result = "Rebels";
                    break;
                case "imperial":
                    result = "Empire";
                    break;
                case "scum":
                    result = "Scum";
                    break;
                default:
                    break;
            }

            return result;
        }

        private static string UpgradeTypeToXWS(UpgradeType upgradeType)
        {
            string result = "";

            switch (upgradeType)
            {
                case UpgradeType.Elite:
                    result = "ept";
                    break;
                case UpgradeType.Astromech:
                    result = "amd";
                    break;
                case UpgradeType.SalvagedAstromech:
                    result = "samd";
                    break;
                case UpgradeType.Modification:
                    result = "mod";
                    break;
                default:
                    result = upgradeType.ToString().ToLower();
                    break;
            }

            return result;
        }

        private static UpgradeType XwsToUpgradeType(string upgradeXws)
        {
            UpgradeType result = UpgradeType.Astromech;

            switch (upgradeXws)
            {
                case "ept":
                    result = UpgradeType.Elite;
                    break;
                case "amd":
                    result = UpgradeType.Astromech;
                    break;
                case "samd":
                    result = UpgradeType.SalvagedAstromech;
                    break;
                case "mod":
                    result = UpgradeType.Modification;
                    break;
                default:
                    string capitalizedName = upgradeXws.First().ToString().ToUpper() + upgradeXws.Substring(1);
                    result = (UpgradeType)Enum.Parse(typeof(UpgradeType), capitalizedName);
                    break;
            }

            return result;
        }*/
    }
}
