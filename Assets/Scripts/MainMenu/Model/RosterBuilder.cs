using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using System.Linq;
using Players;
using Ship;
using Upgrade;
using UnityEngine.UI;
using UnityEngine.Networking;

public static partial class RosterBuilder {

    private class SquadBuilderUpgrade
    {
        public UpgradeSlot Slot;
        public GameObject Panel;

        public SquadBuilderUpgrade(UpgradeSlot slot, GameObject panel)
        {
            Slot = slot;
            Panel = panel;
        }
    }

    private class SquadBuilderShip
    {
        public GenericShip Ship { get; private set; }
        public GameObject Panel;
        public PlayerNo Player;

        private List<SquadBuilderUpgrade> Upgrades = new List<SquadBuilderUpgrade>();

        public SquadBuilderShip(GenericShip ship, GameObject panel, PlayerNo playerNo)
        {
            Ship = ship;
            Panel = panel;
            Player = playerNo;

            InitializeShip();
        }

        public void UpdateShip(GenericShip ship)
        {
            Ship = ship;
            InitializeShip();
        }

        private void InitializeShip()
        {
            Ship.InitializePilotForSquadBuilder();
        }

        public void AddUpgrade(SquadBuilderUpgrade upgradeHolder)
        {
            Upgrades.Add(upgradeHolder);

            UpdateUpgradeDropdowList(upgradeHolder);

            SubscribeUpgradeDropdowns(this, upgradeHolder);
            AddUpgradeTooltip(upgradeHolder.Panel);
        }

        public void RemoveUpgrade(SquadBuilderUpgrade upgradeHolder)
        {
            Upgrades.Remove(upgradeHolder);
            MonoBehaviour.DestroyImmediate(upgradeHolder.Panel);
        }

        public void ChangeUpgradeSlot(SquadBuilderUpgrade upgradeHolder, UpgradeSlot newSlot)
        {
            UpgradeSlot oldUpgradeSlot = upgradeHolder.Slot;

            upgradeHolder.Slot = newSlot;

            UpdateUpgradeDropdowList(upgradeHolder);

            if (oldUpgradeSlot.InstalledUpgrade != null)
            {
                if (Ship.UpgradeBar.HasFreeUpgradeSlot(oldUpgradeSlot.InstalledUpgrade.Type))
                {
                    TryToReinstallUpgrade(this, upgradeHolder, oldUpgradeSlot.InstalledUpgrade);
                }
            }
        }

        private void UpdateUpgradeDropdowList(SquadBuilderUpgrade upgradeHolder)
        {
            List<string> upgradeList = GetUpgradesList(this, upgradeHolder.Slot);
            SetUpgradesDropdown(this, upgradeHolder, upgradeList);
        }

        public List<SquadBuilderUpgrade> GetUpgrades()
        {
            return Upgrades;
        }
    }

    private static class SquadBuilderRoster
    {
        private static List<SquadBuilderShip> roster = new List<SquadBuilderShip>();
        public static Dictionary<PlayerNo, Faction> playerFactions = new Dictionary<PlayerNo, Faction>();

        public static void AddShip(SquadBuilderShip ship)
        {
            roster.Add(ship);
        }

        public static void RemoveShip(SquadBuilderShip ship)
        {
            if (roster.Contains(ship)) roster.Remove(ship);
        }

        public static void RemoveShip(PlayerNo playerNo, GameObject panel)
        {
            SquadBuilderShip ship = roster.Find(n => n.Panel == panel);
            if (ship != null) RemoveShip(ship);
        }

        public static List<SquadBuilderShip> GetShipsByPlayer(PlayerNo playerNo)
        {
            return roster.Where(n => n.Player == playerNo).ToList();
        }

        public static List<SquadBuilderShip> GetShips()
        {
            return roster;
        }

        public static void ClearRoster()
        {
            roster = new List<SquadBuilderShip>();
            playerFactions = new Dictionary<PlayerNo, Faction>();
        }
    }
    

    private static Dictionary<string, string> AllShips = new Dictionary<string, string>();
    private static Dictionary<string, string> AllShipsXws = new Dictionary<string, string>();

    private static Dictionary<string, string> AllPilots = new Dictionary<string, string>();
    private static Dictionary<string, string> AllPilotsXws = new Dictionary<string, string>();
    private static Dictionary<string, int> PilotSkill = new Dictionary<string, int>();

    private static Dictionary<string, string> AllUpgrades = new Dictionary<string, string>();

    public static void Initialize()
    {
        SetPlayers();
        SetPlayerFactions();
        GenerateShipsList();
        AddInitialShips();
    }

    //Initialization

    private static void SetPlayers()
    {
        Global.RemoveAllPlayers();
        Global.AddPlayer(GetPlayerType(PlayerNo.Player1));
        Global.AddPlayer(GetPlayerType(PlayerNo.Player2));
    }

    //TODO: Change to property of Player
    private static void SetPlayerFactions()
    {
        Global.RemoveAllFactions();
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player1));
        Global.AddFaction(GetPlayerFaction(PlayerNo.Player2));
    }

    public static void GeneratePlayersShipConfigurations()
    {
        Global.RemoveAllShips();
        foreach (var ship in SquadBuilderRoster.GetShips())
        {
            ship.Ship.SkinName = GetSkinName(ship);
            Global.AddShip(ship.Ship, ship.Player, GetShipCostCalculated(ship));
        }
        SquadBuilderRoster.ClearRoster();
    }

    private static void AddInitialShips()
    {
        AddInitialShip(PlayerNo.Player1);
        AddInitialShip(PlayerNo.Player2);
    }

    private static void AddInitialShip(PlayerNo playerNo)
    {
        RemoveAllShipsByPlayer(playerNo);
        if (GetShipsCount(playerNo) == 0) TryAddShip(playerNo);
    }

    public static void TryAddShip(PlayerNo playerNo)
    {
        if (GetShipsCount(playerNo) < 8)
        {
            AddShip(playerNo);
        }
        else
        {
            Messages.ShowError("You cannot have more than 8 ships");
        }
    }

    private static SquadBuilderShip AddShip(PlayerNo playerNo)
    {
        List<string> shipResults = GetShipsByFaction(Global.GetPlayerFaction(playerNo));
        string shipNameId = AllShips[shipResults.First()];

        List<string> pilotResults = GetPilotsList(shipNameId, SquadBuilderRoster.playerFactions[playerNo]).OrderByDescending(n => PilotSkill[n]).ToList();
        string pilotId = AllPilots[pilotResults.Last()];
        GenericShip pilot = (GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        GameObject panel = CreateShipPanel(playerNo);

        SquadBuilderShip squadBuilderShip = new SquadBuilderShip(pilot, panel, playerNo);
        SquadBuilderRoster.AddShip(squadBuilderShip);

        SetShipsDropdown(squadBuilderShip, shipResults);
        SetPilotsDropdown(squadBuilderShip, pilotResults);
        SetSkinsDropdown(squadBuilderShip, GetSkins(pilot));
        SetAvailableUpgrades(squadBuilderShip);

        OrganizeUpgradeLines(panel);
        UpdateShipCost(squadBuilderShip);
        OrganizeShipsList(playerNo);

        return squadBuilderShip;
    }

    private static List<string> GetSkins(GenericShip ship)
    {
        List<string> result = new List<string>();

        UnityEngine.Object[] textures = Resources.LoadAll("ShipSkins/" + ship.FixTypeName(ship.Type) + "/");
        foreach (var texture in textures)
        {
            result.Add(texture.name);
        }

        return result;
    }

    private static void ChangeShip(SquadBuilderShip squadBuilderShip)
    {
        squadBuilderShip.UpdateShip(ChangeShipHolder(squadBuilderShip));

        SetSkinsDropdown(squadBuilderShip, GetSkins(squadBuilderShip.Ship));
        UpdateUpgradePanelsFull(squadBuilderShip);
        UpdateShipCost(squadBuilderShip);

        OrganizeShipsList(squadBuilderShip.Player);
    }

    private static GenericShip ChangeShipHolder(SquadBuilderShip squadBuilderShip)
    {
        GenericShip result = null;

        string shipName = GetNameOfShip(squadBuilderShip);
        List<string> pilotResults = GetPilotsList(shipName, SquadBuilderRoster.playerFactions[squadBuilderShip.Player]).OrderByDescending(n => PilotSkill[n]).ToList();

        string pilotId = AllPilots[pilotResults.Last()];
        result = (GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        SetPilotsDropdown(squadBuilderShip, pilotResults);
        //UpdateAvailableUpgrades(squadBuilderShip);
        //OrganizeUpgradeLines(panel);

        return result;
    }

    private static void ChangePilot(SquadBuilderShip squadBuilderShip)
    {
        squadBuilderShip.UpdateShip(ChangePilotHolder(squadBuilderShip));

        //SetAvailableUpgrades(squadBuilderShip);
        //OrganizeUpgradeLines(squadBuilderShip.Panel);

        SetSkinsDropdown(squadBuilderShip, GetSkins(squadBuilderShip.Ship));
        UpdateUpgradePanelsDiff(squadBuilderShip);
        UpdateShipCost(squadBuilderShip);

        OrganizeShipsList(squadBuilderShip.Player);
    }

    private static GenericShip ChangePilotHolder(SquadBuilderShip squadBuilderShip)
    {
        GenericShip result = null;

        string pilotId = GetNameOfPilot(squadBuilderShip);
        result = (GenericShip)Activator.CreateInstance(Type.GetType(pilotId));

        return result;
    }

    private static void ChangeUpgrade(SquadBuilderShip squadBuilderShip, SquadBuilderUpgrade upgrade)
    {
        string upgradeId = GetNameOfUpgrade(upgrade);
        if (upgrade.Slot.InstalledUpgrade != null) upgrade.Slot.RemovePreInstallUpgrade();
        if (!string.IsNullOrEmpty(upgradeId))
        {
            upgrade.Slot.PreInstallUpgrade((GenericUpgrade)Activator.CreateInstance(Type.GetType(upgradeId)), squadBuilderShip.Ship);
        }

        UpdateUpgradePanelsDiff(squadBuilderShip);
    }

    private static List<string> GetShipsByFaction(Faction faction)
    {
        List <string> result = new List<string>();
        foreach (var ships in AllShips)
        {
            GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(ships.Value + "." + ships.Value.Substring(5)));
            if (newShip.factions.Contains(faction))
            {
                result.Add(ships.Key);
            }
        }

        return result;
    }

    // Generate lists of everything

    private static void GenerateShipsList()
    {
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

                if (!AllShips.ContainsKey(newShipTypeContainer.Type))
                {
                    AllShips.Add(newShipTypeContainer.Type, ns);

                    string shipTypeXws = newShipTypeContainer.ShipTypeCanonical;
                    AllShipsXws.Add(shipTypeXws, ns);

                    GetPilotsList(newShipTypeContainer.Type);
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
            if ((newShipContainer.PilotName != null) && (!newShipContainer.IsHidden))
            {
                if ((newShipContainer.faction == faction) || faction == Faction.None)
                {
                    string pilotKey = newShipContainer.PilotName + " (" + newShipContainer.Cost + ")";

                    if (!AllPilots.ContainsKey(pilotKey))
                    {
                        AllPilots.Add(pilotKey, type.ToString());
                        PilotSkill.Add(pilotKey, newShipContainer.PilotSkill);

                        string pilotNameXws = newShipContainer.PilotNameCanonical;
                        AllPilotsXws.Add(pilotNameXws, type.ToString());
                    }

                    result.Add(pilotKey);
                }
                
            }
        }

        return result;
    }

    private static void SetAvailableUpgrades(SquadBuilderShip squadBuilderShip)
    {
        foreach (var upgradeSlot in squadBuilderShip.Ship.UpgradeBar.GetUpgradeSlots())
        {
            AddUpgrade(squadBuilderShip, upgradeSlot);
        }
    }

    private static void AddUpgrade(SquadBuilderShip squadBuilderShip, UpgradeSlot upgradeSlot)
    {
        GameObject panel = CreateUpgradePanel(squadBuilderShip, upgradeSlot);

        SquadBuilderUpgrade upgrade = new SquadBuilderUpgrade(upgradeSlot, panel);
        squadBuilderShip.AddUpgrade(upgrade);
    }

    private static List<string> GetUpgradesList(SquadBuilderShip squadBuilderShip, UpgradeSlot upgradeSlot)
    {
        List<string> results = new List<string>();

        List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => String.Equals(t.Namespace, "UpgradesList", StringComparison.Ordinal))
            .ToList();

        foreach (var type in typelist)
        {
            if (type.MemberType == MemberTypes.NestedType) continue;

            GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(type);
            if (!newUpgrade.IsHidden)
            {
                if (newUpgrade.Type == upgradeSlot.Type && newUpgrade.IsAllowedForShip(squadBuilderShip.Ship) && upgradeSlot.UpgradeIsAllowed(newUpgrade))
                {
                    string upgradeKey = newUpgrade.Name + " (" + (newUpgrade.Cost - upgradeSlot.CostDecrease) + ")";
                    if (!AllUpgrades.ContainsKey(upgradeKey))
                    {
                        AllUpgrades.Add(upgradeKey, type.ToString());
                    }
                    results.Add(upgradeKey);
                }
            }
        }

        return results;
    }

    //Change faction

    public static void PlayerFactonChange()
    {
        SetPlayerFactions();
        CheckPlayerFactonChange(PlayerNo.Player1);
        CheckPlayerFactonChange(PlayerNo.Player2);
    }

    //Go to battle

    public static void StartGame()
    {
        if (!Network.IsNetworkGame)
        {
            StartLocalGame();
        }
        else
        {
            StartNetworkGame();
        }
    }

    private static void StartLocalGame()
    {
        SetPlayers();
        GeneratePlayersShipConfigurations();
        if (ValidatePlayersRosters())
        {
            LoadBattleScene();
        }
    }

    private static void StartNetworkGame()
    {
        //Network.Test();
        //Network.CallBacksTest();

        Network.StartNetworkGame();
    }

    public static void ShowOpponentSquad()
    {
        GameObject globalUI = GameObject.Find("GlobalUI").gameObject;
        MonoBehaviour.DontDestroyOnLoad(globalUI);

        GameObject opponentSquad = globalUI.transform.Find("OpponentSquad").gameObject;
        opponentSquad.SetActive(true);
    }

    public static void HideNetworkManagerHUD()
    {
        GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().showGUI = false;
    }

    public static void LoadBattleScene()
    {
        //TestRandom();
        SceneManager.LoadScene("Battle");
    }

    // TEST

    private static int storedRandomValue;

    private static void TestRandom()
    {
        Network.GenerateRandom(
            new Vector2(1, 10),
            1,
            TestStore,
            TestCallBack
        );
    }

    private static void TestStore(int[] randomHolder)
    {
        storedRandomValue = randomHolder[0];
    }

    private static void TestCallBack()
    {
        Messages.ShowInfo("Random Value is " + storedRandomValue);
    }

    public static string TestGetNameOfFirstShipInRoster()
    {
        string result = "";
        result = SquadBuilderRoster.GetShips()[0].Ship.Type;
        return result;
    }
    
    // END TEST

    private static bool ValidatePlayersRosters()
    {
        if (!ValidatePlayerRoster(PlayerNo.Player1)) return false;
        if (!ValidatePlayerRoster(PlayerNo.Player2)) return false;
        if (!ValidateDifferentUpgradesInAdditionalSlots()) return false;
        return true;
    }

    private static bool ValidatePlayerRoster(PlayerNo playerNo)
    {
        if (!ValidateUniqueCards(playerNo)) return false;
        if (!ValidateSquadCost(playerNo)) return false;
        if (!ValidateShipAiReady(playerNo)) return false;
        return true;
    }

    private static bool ValidateDifferentUpgradesInAdditionalSlots()
    {
        bool result = true;

        foreach (var shipHolder in SquadBuilderRoster.GetShips())
        {
            foreach (var upgradeSlot in shipHolder.Ship.UpgradeBar.GetUpgradeSlots())
            {
                if (upgradeSlot.MustBeDifferent && !upgradeSlot.IsEmpty)
                {
                    int countDuplicates = shipHolder.Ship.UpgradeBar.GetInstalledUpgrades().Count(n => n.Name == upgradeSlot.InstalledUpgrade.Name);
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

    private static bool ValidateSquadCost(PlayerNo playerNo)
    {
        bool result = true;

        int squadCost = 0;

        foreach (var shipConfig in Global.ShipConfigurations)
        {
            if (shipConfig.Player == playerNo)
            {
                squadCost += shipConfig.Ship.Cost;

                foreach (var upgradeSlot in shipConfig.Ship.UpgradeBar.GetUpgradeSlots())
                {
                    if (!upgradeSlot.IsEmpty)
                    {
                        squadCost += upgradeSlot.InstalledUpgrade.Cost - upgradeSlot.CostDecrease;
                    }
                }
            }
        }

        if (!DebugManager.DebugNoSquadPointsLimit)
        {
            if (squadCost > 100)
            {
                Messages.ShowError("Cost of squadron cannot be more than 100");
                result = false;
            }
        }

        return result;
    }

    private static bool ValidateUniqueCards(PlayerNo playerNo)
    {
        bool result = true;

        List<string> uniqueCards = new List<string>();
        foreach (var shipConfig in Global.ShipConfigurations)
        {
            if (shipConfig.Player == playerNo)
            {
                if (shipConfig.Ship.IsUnique)
                {
                    if (CheckDuplicate(uniqueCards, shipConfig.Ship.PilotName)) return false;
                }

                foreach (var upgrade in shipConfig.Ship.UpgradeBar.GetInstalledUpgrades())
                {
                    if (upgrade.isUnique)
                    {
                        if (CheckDuplicate(uniqueCards, upgrade.Name)) return false;
                    }
                }
            }
        }
        return result;
    }

    private static bool CheckDuplicate(List<string> uniqueCards, string cardName)
    {
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

    private static bool ValidateShipAiReady(PlayerNo playerNo)
    {
        bool result = true;

        foreach (var shipConfig in Global.ShipConfigurations)
        {
            if (shipConfig.Player == playerNo && GetPlayerType(playerNo) == typeof(HotacAiPlayer))
            {
                if (shipConfig.Ship.HotacManeuverTable == null)
                {
                    Messages.ShowError("AI for " + shipConfig.Ship.Type + " is not ready. It can be controlled only by human.");
                    return false;
                }
            }
        }

        return result;
    }

    private static void TryToReinstallUpgrade(SquadBuilderShip squadBuilderShip, SquadBuilderUpgrade squadUpgrade, GenericUpgrade oldUpgrade)
    {
        if (oldUpgrade.IsAllowedForShip(squadBuilderShip.Ship) && squadUpgrade.Slot.UpgradeIsAllowed(oldUpgrade))
        {
            Dropdown upgradeDropbox = squadUpgrade.Panel.transform.GetComponent<Dropdown>();
            string upgradeDropboxName = AllUpgrades.Where(n => n.Value == oldUpgrade.GetType().ToString()).First().Key;

            bool isFound = false;
            for (int i = 0; i < upgradeDropbox.options.Count; i++)
            {
                if (upgradeDropbox.options[i].text == upgradeDropboxName)
                {
                    upgradeDropbox.value = i;
                    squadUpgrade.Slot.PreInstallUpgrade(oldUpgrade, squadBuilderShip.Ship);
                    isFound = true;
                    break;
                }
            }
            if (!isFound) upgradeDropbox.value = 0;
        }
    }

    // IMPORT / EXPORT

    public static void ImportSquadList()
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

    private static void RemoveAllShipsByPlayer(PlayerNo playerNo)
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
                string shipNamePath = AllShipsXws[shipNameXws];
                string shipNameGeneral = AllShips.Where(n => n.Value == shipNamePath).First().Key;
                Dropdown shipDropdown = newShip.Panel.transform.Find("GroupShip/DropdownShip").GetComponent<Dropdown>();
                shipDropdown.value = shipDropdown.options.IndexOf(shipDropdown.options.Find(n => n.text == shipNameGeneral));

                OnShipChanged(newShip);

                string pilotNameXws = pilotJson["name"].str;
                string pilotNamePath = AllPilotsXws[pilotNameXws];
                string pilotNameGeneral = AllPilots.Where(n => n.Value == pilotNamePath).First().Key;
                Dropdown pilotDropdown = newShip.Panel.transform.Find("GroupShip/DropdownPilot").GetComponent<Dropdown>();
                pilotDropdown.value = pilotDropdown.options.IndexOf(pilotDropdown.options.Find(n => n.text == pilotNameGeneral));

                OnPilotChanged(newShip);
            }
        }
        else
        {
            Messages.ShowError("No pilots");
        }

        callBack();
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
            case Faction.Rebels:
                result = "rebel";
                break;
            case Faction.Empire:
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

}
