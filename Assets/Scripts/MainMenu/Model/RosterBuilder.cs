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
using GameModes;

public static partial class RosterBuilder {

    public class SquadBuilderUpgrade
    {
        public SquadBuilderShip Host;
        public UpgradeSlot Slot;
        public GameObject Panel;

        public SquadBuilderUpgrade(SquadBuilderShip host, UpgradeSlot slot, GameObject panel)
        {
            Host = host;
            Slot = slot;
            Panel = panel;
        }
    }

    public class SquadBuilderShip
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

                if (!oldUpgradeSlot.InstalledUpgrade.IsAllowedForShip(oldUpgradeSlot.InstalledUpgrade.Host))
                {
                    oldUpgradeSlot.RemovePreInstallUpgrade();
                    upgradeHolder.Panel.GetComponentInChildren<Dropdown>().value = 0;
                    UpdateUpgradePanelsDiff(this);
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

    public static class SquadBuilderRoster
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

    public class ShipRecord
    {
        public string ShipName;
        public string ShipNameCanonical;
        public string ShipNamespace;
    }

    public class PilotRecord
    {
        public string PilotName;
        public string PilotNameWithCost;
        public string PilotTypeName;
        public string PilotNameCanonical;
        public int PilotSkill;
        public ShipRecord PilotShip;
    }

    public class UpgradeRecord
    {
        public string UpgradeName;
        public string UpgradeNameWithCost;
        public string UpgradeNameCanonical;
        public string UpgradeTypeName;
    }

    public static List<PilotRecord> AllPilots = new List<PilotRecord>();
    public static List<ShipRecord> AllShips = new List<ShipRecord>();
    public static List<UpgradeRecord> AllUpgrades = new List<UpgradeRecord>();

    public static void Initialize()
    {
        InitializeSquadBuilderRoster();
        SetPlayers();
        SetPlayerFactions();
        GenerateShipsList();
        GenerateUpgradesList();
        AddInitialShips();
    }

    //Initialization

    private static void InitializeSquadBuilderRoster()
    {
        RemoveAllShips();
        SquadBuilderRoster.ClearRoster();
    }

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
        ShipFactory.Initialize();
        Global.RemoveAllShips();

        foreach (var ship in SquadBuilderRoster.GetShips())
        {
            ship.Ship.SkinName = GetSkinName(ship);
            Global.AddShip(ship.Ship, ship.Player, GetShipCostCalculated(ship));
        }
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
        string shipNameId = AllShips.Find(n => n.ShipName == shipResults.First()).ShipNamespace;

        List<string> pilotResults = GetPilotsList(shipNameId, SquadBuilderRoster.playerFactions[playerNo]).OrderByDescending(n => AllPilots.Find(m => m.PilotNameWithCost == n).PilotSkill).ToList();
        string pilotId = AllPilots.Find(n => n.PilotNameWithCost == pilotResults.Last()).PilotTypeName;
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

        string shipNameId = GetNameOfShip(squadBuilderShip);
        List<string> pilotResults = GetPilotsList(shipNameId, SquadBuilderRoster.playerFactions[squadBuilderShip.Player]).OrderByDescending(n => AllPilots.Find(m => m.PilotNameWithCost == n).PilotSkill).ToList();

        string pilotId = AllPilots.Find(n => n.PilotNameWithCost == pilotResults.Last()).PilotTypeName;
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
        foreach (var ship in AllShips)
        {
            GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(ship.ShipNamespace + "." + ship.ShipNamespace.Substring(5)));
            //newShip.CheckAITable();
            if (newShip.factions.Contains(faction))
            {
                result.Add(ship.ShipName);
            }
        }

        return result;
    }

    // Generate lists of everything

    private static void GenerateShipsList()
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
                    AllShips.Add(new ShipRecord() {
                        ShipName = newShipTypeContainer.Type,
                        ShipNameCanonical = newShipTypeContainer.ShipTypeCanonical,
                        ShipNamespace = ns
                    });

                    GetPilotsList(ns);
                }
            }
        }

        //Messages.ShowInfo("Ships loaded: " + AllShips.Count);
        //Messages.ShowInfo("Pilots loaded: " + AllPilots.Count);
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
                            PilotNameWithCost = pilotKey,
                            PilotTypeName = type.ToString(),
                            PilotNameCanonical = newShipContainer.PilotNameCanonical,
                            PilotSkill = newShipContainer.PilotSkill,
                            PilotShip = AllShips.Find(n => n.ShipName == newShipContainer.Type)
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
                        UpgradeNameWithCost = newUpgradeContainer.Name + " (" + newUpgradeContainer.Cost + ")",
                        UpgradeTypeName = type.ToString()
                    });
                }
            }
        }

        //Messages.ShowInfo("Upgrades loaded: " + AllUpgrades.Count);
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

        SquadBuilderUpgrade upgrade = new SquadBuilderUpgrade(squadBuilderShip, upgradeSlot, panel);
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
            if (newUpgrade.IsAllowedForSquadBuilder())
            {
                if (newUpgrade.Type == upgradeSlot.Type && newUpgrade.IsAllowedForShip(squadBuilderShip.Ship) && upgradeSlot.UpgradeIsAllowed(newUpgrade))
                {
                    string upgradeKey = newUpgrade.Name + " (" + ReduceUpgradeCost(newUpgrade.Cost, upgradeSlot.CostDecrease) + ")";
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

    public static void StartLocalGame()
    {
        GameMode.CurrentGameMode = new LocalGame();

        SetPlayers();
        GeneratePlayersShipConfigurations();
        if (ValidatePlayersRosters())
        {
            LoadBattleScene();
        }
    }

    public static void StartNetworkGame()
    {
        //Network.Test();
        //Network.CallBacksTest();

        Network.StartNetworkGame();
    }

    public static void ShowOpponentSquad()
    {
        GameObject globalUI = GameObject.Find("GlobalUI").gameObject;

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
        SquadBuilderRoster.ClearRoster();

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
        if (!ValidateLimitedCards(playerNo)) return false;
        if (!ValidateShipAiReady(playerNo)) return false;
        if (!ValidateUpgradePostChecks(playerNo)) return false;
        return true;
    }

    private static bool ValidateUpgradePostChecks(PlayerNo playerNo)
    {
        bool result = true;

        foreach (var shipHolder in SquadBuilderRoster.GetShips())
        {
            foreach (var upgradeHolder in shipHolder.GetUpgrades())
            {
                if (!upgradeHolder.Slot.IsEmpty)
                {
                    if (!upgradeHolder.Slot.InstalledUpgrade.IsAllowedForSquadBuilderPostCheck(upgradeHolder)) return false;
                }
            }
        }

        return result;
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
                        squadCost += ReduceUpgradeCost(upgradeSlot.InstalledUpgrade.Cost, upgradeSlot.CostDecrease);
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

    private static bool ValidateLimitedCards(PlayerNo playerNo)
    {
        bool result = true;

        foreach (var shipConfig in Global.ShipConfigurations)
        {
            if (shipConfig.Player == playerNo)
            {
                List<string> limitedCards = new List<string>();

                foreach (var upgrade in shipConfig.Ship.UpgradeBar.GetInstalledUpgrades())
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
        }

        return result;
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
            string upgradeDropboxName = AllUpgrades.Where(n => n.UpgradeTypeName == oldUpgrade.GetType().ToString()).First().UpgradeNameWithCost;

            bool isFound = false;
            for (int i = 0; i < upgradeDropbox.options.Count; i++)
            {
                if (upgradeDropbox.options[i].text == upgradeDropboxName)
                {
                    upgradeDropbox.value = i;
                    if (squadUpgrade.Slot.InstalledUpgrade != null) squadUpgrade.Slot.RemovePreInstallUpgrade();
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
                result = (UpgradeType) Enum.Parse(typeof(UpgradeType), capitalizedName);
                break;
        }

        return result;
    }

}
