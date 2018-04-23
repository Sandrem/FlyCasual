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

        public SquadList(PlayerNo playerNo)
        {
            PlayerNo = playerNo;
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
                if ((newShipContainer.PilotName != null) && (newShipContainer.IsAllowedForSquadBuilder()))
                {
                    if ((newShipContainer.faction == faction) || faction == Faction.None)
                    {
                        string pilotKey = newShipContainer.PilotName + " (" + newShipContainer.Cost + ")";

                        if (AllPilots.Find(n => n.PilotName == newShipContainer.PilotName && n.PilotShip.ShipName == newShipContainer.Type && n.PilotFaction == newShipContainer.faction) == null)
                        {
                            AllPilots.Add(new PilotRecord()
                            {
                                PilotName = newShipContainer.PilotName,
                                PilotTypeName = type.ToString(),
                                PilotNameCanonical = newShipContainer.PilotNameCanonical,
                                PilotSkill = newShipContainer.PilotSkill,
                                PilotShip = AllShips.Find(n => n.ShipName == newShipContainer.Type),
                                PilotFaction = newShipContainer.faction,
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

            //Messages.ShowInfo("Upgrades: " + AllUpgrades.Count);
        }

        public static void SetCurrentPlayerFaction(Faction faction)
        {
            CurrentSquadList.SquadFaction = faction;
        }

        public static void ShowShipsFilteredByFaction()
        {
            ShowAvailableShips(CurrentSquadList.SquadFaction);
        }

        public static void ShowPilotsFilteredByShipAndFaction()
        {
            ShowAvailablePilots(CurrentSquadList.SquadFaction, CurrentShip);
        }

        private static SquadBuilderShip AddPilotToSquad(GenericShip ship, PlayerNo playerNo)
        {
            return GetSquadList(playerNo).AddShip(ship);
        }

        private static void InstallUpgrade(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            slot.PreInstallUpgrade(upgrade, CurrentSquadBuilderShip.Instance);
        }

        private static void InstallUpgrade(SquadBuilderShip ship, string upgradeName)
        {
            string upgradeType = AllUpgrades.Find(n => n.UpgradeName == upgradeName).UpgradeTypeName;
            GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));

            List<UpgradeSlot> slots = FindFreeSlots(ship, newUpgrade.Types);
            slots[0].PreInstallUpgrade(newUpgrade, ship.Instance);
        }

        private static List<UpgradeSlot> FindFreeSlots(SquadBuilderShip shipHolder, List<UpgradeType> upgradeTypes)
        {
            return shipHolder.Instance.UpgradeBar.GetFreeSlots (upgradeTypes);
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
            UpdateSkinButton();
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

            result += shipHolder.Instance.Cost;

            foreach (var upgradeSlot in shipHolder.Instance.UpgradeBar.GetUpgradeSlots())
            {
                if (!upgradeSlot.IsEmpty)
                {
                    result += ReduceUpgradeCost(upgradeSlot.InstalledUpgrade.Cost, upgradeSlot.CostDecrease);
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
            GetSquadList(PlayerNo.Player1).PlayerType = playerOneType;
            GetSquadList(PlayerNo.Player2).PlayerType = playerTwoType;
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
            SwitchToBattlecene();
        }

        public static void SwitchToBattlecene()
        {
            Global.ToggelLoadingScreen(true);
            LoadBattleScene();
        }

        public static void SaveSquadConfigurations()
        {
            foreach (var squad in SquadLists)
            {
                squad.SavedConfiguration = GetSquadInJson(squad.PlayerNo);

                JSONObject playerInfoJson = new JSONObject();
                playerInfoJson.AddField("NickName", Options.NickName);
                playerInfoJson.AddField("Title", Options.Title);
                playerInfoJson.AddField("Avatar", Options.Avatar);
                squad.SavedConfiguration.AddField("PlayerInfo", playerInfoJson);

                ClearShipsOfPlayer(squad.PlayerNo);
            }
        }

        public static void LoadBattleScene()
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
            if (!ValidateSlotsRequirements(playerNo)) return false;

            return true;
        }

        private static bool RosterIsEmpty(PlayerNo playerNo)
        {
            bool result = false;
            if (GetSquadList(playerNo).GetShips().Count == 0)
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
            foreach (var shipConfig in GetSquadList(playerNo).GetShips())
            {
                if (shipConfig.Instance.IsUnique)
                {
                    if (CheckDuplicate(uniqueCards, shipConfig.Instance.PilotName)) return false;
                }

                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
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

            foreach (var shipConfig in GetSquadList(playerNo).GetShips())
            {
                List<string> limitedCards = new List<string>();

                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
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
            SquadList aiSquadlist = GetSquadList(playerNo);
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

            SquadList squadList = GetSquadList(playerNo);

            foreach (var shipHolder in squadList.GetShips())
            {
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
                            int countDuplicates = shipHolder.Instance.UpgradeBar.GetUpgradesAll().Count(n => n.Name == upgradeSlot.InstalledUpgrade.Name);
                            if (countDuplicates > 1)
                            {
                                Messages.ShowError("Upgrades must be different: " + upgradeSlot.InstalledUpgrade.Name);
                                return false;
                            }
                        }
                        if (upgradeSlot.InstalledUpgrade.Cost > upgradeSlot.MaxCost)
                        {
                            Messages.ShowError("Upgrade must costs less than " + upgradeSlot.MaxCost + " : " + upgradeSlot.InstalledUpgrade.Name);
                            return false;
                        }
                        if (upgradeSlot.MustBeUnique && !upgradeSlot.InstalledUpgrade.isUnique)
                        {
                            Messages.ShowError("Upgrade must be unique : " + upgradeSlot.InstalledUpgrade.Name);
                            return false;
                        }
                    }                
                }
            }

            return result;
        }

        private static List<string> GetAvailableShipSkins(SquadBuilderShip ship)
        {
            List<string> result = new List<string>();

            UnityEngine.Object[] textures = Resources.LoadAll("ShipSkins/" + ship.Instance.FixTypeName(ship.Instance.Type) + "/");
            foreach (var texture in textures)
            {
                result.Add(texture.name);
            }

            return result;
        }

        // IMPORT / EXPORT

        public static void CreateSquadFromImportedJson(string jsonString, PlayerNo playerNo, Action callback)
        {
            JSONObject squadJson = new JSONObject(jsonString);
            //LogImportedSquad(squadJson);

            SetPlayerSquadFromImportedJson(
                squadJson,
                playerNo,
                callback
            );
        }

        public static void SetPlayerSquadFromImportedJson(JSONObject squadJson, PlayerNo playerNo, Action callBack)
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
                Faction faction = XWSToFaction(factionNameXws);
                squadList.SquadFaction = faction;

                squadList.Points = (int) squadJson["points"].i;

                if (squadJson.HasField("pilots"))
                {
                    JSONObject pilotJsons = squadJson["pilots"];
                    foreach (JSONObject pilotJson in pilotJsons.list)
                    {
                        string shipNameXws = pilotJson["ship"].str;
                        string shipNameGeneral = AllShips.Find(n => n.ShipNameCanonical == shipNameXws).ShipName;

                        string pilotNameXws = pilotJson["name"].str;
                        string pilotNameGeneral = AllPilots.Find(n => n.PilotNameCanonical == pilotNameXws).PilotName;

                        PilotRecord pilotRecord = AllPilots.Find(n => n.PilotName == pilotNameGeneral && n.PilotShip.ShipName == shipNameGeneral && n.PilotFaction == faction);
                        GenericShip newShipInstance = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
                        SquadBuilderShip newShip = AddPilotToSquad(newShipInstance, playerNo);

                        JSONObject upgradeJsons = pilotJson["upgrades"];
                        foreach (string upgradeType in upgradeJsons.keys)
                        {
                            JSONObject upgradeNames = upgradeJsons[upgradeType];
                            foreach (JSONObject upgradeRecord in upgradeNames.list)
                            {
                                string upgradeName = AllUpgrades.Find(n => n.UpgradeNameCanonical == upgradeRecord.str).UpgradeName;
                                InstallUpgrade(newShip, upgradeName);
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
                                    newShip.Instance.SkinName = myVendorData["skin"].str;
                                }
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
            catch (Exception)
            {
                Messages.ShowError("Error during creation of squadron");
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
            squadJson.AddField("faction", FactionToXWS(GetSquadList(playerNo).SquadFaction));
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

                    string pilotNameXws = pilotJson["name"].str;
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
            pilotJson.AddField("name", shipHolder.Instance.PilotNameCanonical);
            pilotJson.AddField("points", GetShipCost(shipHolder));
            pilotJson.AddField("ship", shipHolder.Instance.ShipTypeCanonical);

            Dictionary<string, JSONObject> upgradesDict = new Dictionary<string, JSONObject>();
            foreach (var installedUpgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
            {
                string slotName = UpgradeTypeToXWS(installedUpgrade.Types[0]);
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

        private static Faction XWSToFaction(string factionXWS)
        {
            Faction result = Faction.None;

            switch (factionXWS)
            {
                case "rebel":
                    result = Faction.Rebel;
                    break;
                case "imperial":
                    result = Faction.Imperial;
                    break;
                case "scum":
                    result = Faction.Scum;
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
        }

        public static bool IsNetworkGame
        {
            get { return GetSquadList(PlayerNo.Player2).PlayerType == typeof(NetworkOpponentPlayer); }
        }

        public static bool IsVsAiGame
        {
            get { return GetSquadList(PlayerNo.Player2).PlayerType == typeof(HotacAiPlayer); }
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
            return shipHolder.Instance.SkinName;
        }

        public static void BrowseSavedSquads()
        {
            ShowListOfSavedSquadrons(GetSavedSquadsJsons());
        }

        private static void DeleteSavedSquadAndRefresh(string fileName)
        {
            DeleteSavedSquadFile(fileName);
            BrowseSavedSquads();
        }

        private static void LoadSavedSquadAndReturn(string fileName)
        {
            JSONObject squadJson = GetSavedSquadJson(fileName);
            SetPlayerSquadFromImportedJson(squadJson, CurrentPlayer, ReturnToSquadBuilder);
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
            else if (playerOneType == typeof(HumanPlayer) && playerTwoType == typeof(HotacAiPlayer))
            {
                if (playerNo == PlayerNo.Player1) return "My Squadron"; else return "Squadron of AI";
            }
            else if (playerOneType == typeof(HotacAiPlayer) && playerTwoType == typeof(HotacAiPlayer))
            {
                if (playerNo == PlayerNo.Player1) return "Squadron of AI 1"; else return "Squadron of AI 2";
            }

            return result;
        }

        private static void SetSkinForShip(SquadBuilderShip ship, string skinName)
        {
            ship.Instance.SkinName = skinName;
        }

        public static void ReGenerateSquads(Action callback)
        {
            ReGenerateSquadOfPlayer(PlayerNo.Player1, delegate { ReGenerateSquadOfPlayer(PlayerNo.Player2, callback); });
        }

        private static void ReGenerateSquadOfPlayer(PlayerNo playerNo, Action callback)
        {
            JSONObject playerJson = GetSquadList(playerNo).SavedConfiguration;
            SetPlayerSquadFromImportedJson(playerJson, playerNo, callback);
        }
    }
}
