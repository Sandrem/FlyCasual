using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Players;
using System.Linq;
using Ship;
using System;

public static partial class Roster {

    private static List<GameObject> rosterPlayer1;
    private static List<GameObject> rosterPlayer2;

    private static int SHIP_PANEL_WIDTH = 300;
    private static int SHIP_PANEL_HEIGHT = 110;
    private static int SPACE_BETWEEN_PANELS = 15;

    public static void Initialize()
    {
        Players = new List<GenericPlayer>();
        rosterPlayer1 = new List<GameObject>();
        rosterPlayer2 = new List<GameObject>();
        AllShips = new Dictionary<string, GenericShip>();
    }
    
    public static GameObject CreateRosterInfo(GenericShip newShip)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/RosterPanel", typeof(GameObject));

        int playerPanelNum = (Network.IsNetworkGame && !Network.IsServer) ? AnotherPlayer(newShip.Owner.Id) : newShip.Owner.Id;

        GameObject newPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/RostersHolder").transform.Find("TeamPlayer" + playerPanelNum).Find("RosterHolder").transform);

        //Generic info
        newPanel.transform.Find("ShipInfo/ShipPilotSkillText").GetComponent<Text>().text = newShip.PilotSkill.ToString();

        newPanel.transform.Find("ShipInfo/ShipFirepowerText").GetComponent<Text>().text = newShip.Firepower.ToString();
        newPanel.transform.Find("ShipInfo/ShipAgilityText").GetComponent<Text>().text = newShip.Agility.ToString();
        newPanel.transform.Find("ShipInfo/ShipHullText").GetComponent<Text>().text = newShip.MaxHull.ToString();
        newPanel.transform.Find("ShipInfo/ShipShieldsText").GetComponent<Text>().text = newShip.MaxShields.ToString();

        // ALT ShipId text
        PlayerNo rosterPanelOwner = (Network.IsNetworkGame && !Network.IsServer) ? AnotherPlayer(newShip.Owner.PlayerNo) : newShip.Owner.PlayerNo;
        newPanel.transform.Find("ShipInfo/ShipId").GetComponent<Text>().text = newShip.ShipId.ToString();
        newPanel.transform.Find("ShipIdText/Text").GetComponent<Text>().text = newShip.ShipId.ToString();
        newPanel.transform.Find("ShipIdText/Text").GetComponent<Text>().color = (newShip.Owner.PlayerNo == PlayerNo.Player1) ? Color.green : Color.red;
        newPanel.transform.Find("ShipIdText").localPosition = new Vector3((rosterPanelOwner == PlayerNo.Player1) ? SHIP_PANEL_WIDTH + 5 : -50, 0, 0);
        newPanel.transform.Find("ShipIdText").gameObject.SetActive(true);

        //Tooltips
        GameObject pilotNameGO = newPanel.transform.Find("ShipInfo/ShipPilotNameText").gameObject;
        pilotNameGO.GetComponent<Text>().text = newShip.PilotName;
        Tooltips.AddTooltip(pilotNameGO, newShip.ImageUrl);
        SubscribeSelectionByInfoPanel(pilotNameGO);

        GameObject shipTypeGO = newPanel.transform.Find("ShipInfo/ShipTypeText").gameObject;
        shipTypeGO.GetComponent<Text>().text = newShip.Type;
        Tooltips.AddTooltip(shipTypeGO, newShip.ManeuversImageUrl);
        SubscribeSelectionByInfoPanel(shipTypeGO);

        //Mark
        newPanel.transform.Find("Mark").localPosition = new Vector3((rosterPanelOwner == PlayerNo.Player1) ? SHIP_PANEL_WIDTH - 2 : -8, 0, 0);
        SubscribeMarkByHover(newPanel);

        //Hull and shields
        float panelWidth = SHIP_PANEL_WIDTH - 10;
        float hullAndShield = newShip.MaxHull + newShip.MaxShields;
        float panelWidthNoDividers = panelWidth - (1 * (hullAndShield - 1));
        float damageIndicatorWidth = panelWidthNoDividers / hullAndShield;

        GameObject damageIndicatorBar = newPanel.transform.Find("ShipInfo/DamageBarPanel").gameObject;
        GameObject damageIndicator = damageIndicatorBar.transform.Find("DamageIndicator").gameObject;
        damageIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(damageIndicatorWidth, 10);
        for (int i = 0; i < hullAndShield; i++)
        {
            GameObject newDamageIndicator = MonoBehaviour.Instantiate(damageIndicator, damageIndicatorBar.transform);
            newDamageIndicator.transform.position = damageIndicator.transform.position + new Vector3(i * (damageIndicatorWidth + 1), 0, 0);
            if (i < newShip.MaxHull) {
                newDamageIndicator.GetComponent<Image>().color = Color.yellow;
                newDamageIndicator.name = "DamageIndicator.Hull." + (i+1).ToString();
            } else
            {
                newDamageIndicator.GetComponent<Image>().color = new Color(0, 202, 255);
                newDamageIndicator.name = "DamageIndicator.Shield." + (i-newShip.MaxHull+1).ToString();
            }
            newDamageIndicator.SetActive(true);
        }
        MonoBehaviour.Destroy(damageIndicator);

        //Assigned Maneuver Dial
        GameObject maneuverDial = newPanel.transform.Find("AssignedManeuverDial").gameObject;
        SubscribeShowManeuverByHover(maneuverDial);
        maneuverDial.transform.localPosition = (rosterPanelOwner == PlayerNo.Player1) ? new Vector3(320, -5, 0) : new Vector3(-120, -5, 0);

        //Tags
        newPanel.transform.Find("ShipInfo").tag = "ShipId:" + newShip.ShipId.ToString();
        maneuverDial.transform.Find("Holder").gameObject.tag = "ShipId:" + newShip.ShipId.ToString();

        //Finish
        AddToRoster(newShip, newPanel);
        newPanel.transform.Find("ShipInfo").gameObject.SetActive(true);

        return newPanel;
    }

    public static void SubscribeShowManeuverByHover(GameObject panel)
    {
        panel.AddComponent<EventTrigger>();

        EventTrigger trigger = panel.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { ShowAssignedManeuverByHover((PointerEventData)data); });
        trigger.triggers.Add(entry);

        trigger = panel.GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { HideAssignedManeuverByHover((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    public static void SubscribeSelectionByInfoPanel(GameObject panel)
    {
        panel.AddComponent<EventTrigger>();
        EventTrigger trigger = panel.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { SelectShipByRosterClick((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    private static void AddToRoster(GenericShip newShip, GameObject newPanel)
    {

        List<GameObject> rosterPlayer = (newShip.Owner.PlayerNo == PlayerNo.Player1) ? rosterPlayer1 : rosterPlayer2;
        rosterPlayer.Add(newPanel);

        OrganizeRosters();

    }

    //ORGANIZE

    public static void OrganizeRosters()
    {
        OrganizeRosterPanelSizes();
        OrganizeRosterPositions();
    }

    private static void OrganizeRosterPanelSizes()
    {
        foreach (GameObject panel in rosterPlayer1)
        {
            panel.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta = new Vector2(SHIP_PANEL_WIDTH, CalculateRosterPanelSize(panel));
        }

        //same
        foreach (GameObject panel in rosterPlayer2)
        {
            panel.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta = new Vector2(SHIP_PANEL_WIDTH, CalculateRosterPanelSize(panel));
        }

    }

    private static int CalculateRosterPanelSize(GameObject panel)
    {
        //Upgrades
        int currentPanelHeight = SHIP_PANEL_HEIGHT;

        int upgradesVisible = 0;

        foreach (Transform icon in panel.transform.Find("ShipInfo/UpgradesBar").transform)
        {
            if (icon.gameObject.activeSelf)
            {
                upgradesVisible++;
            }
        }

        int upgdaresHeight = upgradesVisible * 20;
        currentPanelHeight += upgdaresHeight;

        //Tokens

        panel.transform.Find("ShipInfo/TokensBar").GetComponent<RectTransform>().localPosition = new Vector2(5, -97 - upgdaresHeight);

        int iconsCount = 0;
        foreach (Transform icon in panel.transform.Find("ShipInfo/TokensBar").transform)
        {
            if (icon.gameObject.activeSelf)
            {
                iconsCount++;
            }
        }

        int iconsLines = (iconsCount + 7) / 8;
        currentPanelHeight += 35 * iconsLines + 3;

        panel.transform.Find("Mark").GetComponent<RectTransform>().sizeDelta = new Vector2(10, currentPanelHeight);

        return currentPanelHeight;
    }

    private static void OrganizeRosterPositions()
    {
        for (int i = 1; i < 3; i++)
        {
            Vector3 defaultPosition = GameObject.Find("UI/RostersHolder").transform.Find("TeamPlayer" + i + "/RosterHolder").transform.position + new Vector3(5f, 0f, 0f);

            int rosterPanelOwner = (Network.IsNetworkGame && !Network.IsServer) ? AnotherPlayer(i) : i;
            List<GameObject> rosterPlayer = (rosterPanelOwner == 1) ? rosterPlayer1 : rosterPlayer2;

            rosterPlayer = rosterPlayer
                .OrderByDescending(x => x.transform.Find("ShipInfo/ShipPilotSkillText").GetComponent<Text>().text)
                .ThenBy(x => x.transform.Find("ShipInfo/ShipId").GetComponent<Text>().text)
                .ToList();

            float offset = 5;
            foreach (var item in rosterPlayer)
            {
                if (item.activeSelf)
                {
                    item.transform.position = defaultPosition + new Vector3(0f, -offset, 0f);
                    offset += item.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta.y + SPACE_BETWEEN_PANELS;
                }
            }
        }
    }

    // RMB is not supported
    public static void SelectShipByRosterClick(PointerEventData data)
    {
        foreach (var item in data.hovered)
        {
            if (item.tag != "Untagged")
            {
                if (Roster.AllShips.ContainsKey(item.tag))
                {
                    if (Selection.TryToChangeShip(item.tag)) return;
                }
            }
        }
        UI.HideTemporaryMenus();
    }

    public static void ShowAssignedManeuverByHover(PointerEventData data)
    {
        foreach (var item in data.hovered)
        {
            if (item.tag != "Untagged")
            {
                if (!Network.IsNetworkGame)
                {
                    if (AllShips[item.tag].Owner.PlayerNo == Phases.CurrentPhasePlayer)
                    {
                        ToggelManeuverVisibility(AllShips[item.tag], true);
                        return;
                    }
                }
                else
                {
                    if (AllShips[item.tag].Owner.GetType() == typeof(HumanPlayer))
                    {
                        ToggelManeuverVisibility(AllShips[item.tag], true);
                        return;
                    }
                }
            }
        }
    }

    public static void HideAssignedManeuverByHover(PointerEventData data)
    {
        foreach (var shipHoler in AllShips)
        {
            if (IsAssignedManeuverDialShouldBeHiddenAfterHover(shipHoler.Value))
            {
                ToggelManeuverVisibility(shipHoler.Value, false);
            }
        }
    }

    private static bool IsAssignedManeuverDialShouldBeHiddenAfterHover(GenericShip ship)
    {
        bool result = true;

        if (GetPlayer(AnotherPlayer(ship.Owner.PlayerNo)).GetType() == typeof(HotacAiPlayer)) return false;
        if (GetPlayer(AnotherPlayer(ship.Owner.PlayerNo)).GetType() == typeof(NetworkPlayer)) return false;
        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase)) return false;
        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.MovementExecutionSubPhase)) return false;

        return result;
    }

    public static void SubscribeMarkByHover(GameObject sender)
    {
        EventTrigger trigger = sender.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            sender.AddComponent<EventTrigger>();
            trigger = sender.GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { HoverShipByRosterClick((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { Selection.TryUnmarkPreviousHoveredShip(); });
        trigger.triggers.Add(entry);
    }

    public static void HoverShipByRosterClick(PointerEventData data)
    {
        foreach (var item in data.hovered)
        {
            if (item.tag.StartsWith("ShipId:"))
            {
                if (Roster.AllShips.ContainsKey(item.tag))
                {
                    Selection.TryMarkShip(item.tag);
                }
            }
        }
    }

    //UPDATE

    public static void UpdateRosterShieldsDamageIndicators(GenericShip thisShip)
    {
        thisShip.InfoPanel.transform.Find("ShipInfo/ShipShieldsText").GetComponent<Text>().text = thisShip.Shields.ToString();
        foreach (Transform damageIndicator in thisShip.InfoPanel.transform.Find("ShipInfo/DamageBarPanel").transform)
        {
            string[] damageIndicatorData = damageIndicator.name.Split('.');
            string type = damageIndicatorData[1];
            int value = int.Parse(damageIndicatorData[2]);
            if (type == "Shield")
            {
                damageIndicator.gameObject.SetActive(value <= thisShip.Shields);
            }
        }
    }

    public static void UpdateShipStats(GenericShip thisShip)
    {
        if (thisShip.InfoPanel != null)
        {
            thisShip.InfoPanel.transform.Find("ShipInfo/ShipPilotNameText").GetComponent<Text>().text = thisShip.PilotName;
            thisShip.InfoPanel.transform.Find("ShipInfo/ShipPilotSkillText").GetComponent<Text>().text = thisShip.PilotSkill.ToString();
            thisShip.InfoPanel.transform.Find("ShipInfo/ShipFirepowerText").GetComponent<Text>().text = thisShip.Firepower.ToString();
            thisShip.InfoPanel.transform.Find("ShipInfo/ShipAgilityText").GetComponent<Text>().text = thisShip.Agility.ToString();
        }
    }

    public static void UpdateRosterHullDamageIndicators(GenericShip thisShip)
    {
        thisShip.InfoPanel.transform.Find("ShipInfo/ShipHullText").GetComponent<Text>().text = thisShip.Hull.ToString();
        foreach (Transform damageIndicator in thisShip.InfoPanel.transform.Find("ShipInfo/DamageBarPanel").transform)
        {
            string[] damageIndicatorData = damageIndicator.name.Split('.');
            string type = damageIndicatorData[1];
            int value = int.Parse(damageIndicatorData[2]);
            if (type == "Hull")
            {
                damageIndicator.gameObject.SetActive(value <= thisShip.Hull);
            }
        }

        //Todo: move
        thisShip.ToggleDamaged(thisShip.Hull == 1);
    }

    public static void UpdateTokensIndicator(GenericShip thisShip, System.Type type)
    {
        List<GameObject> keys = new List<GameObject>();
        foreach (Transform icon in thisShip.InfoPanel.transform.Find("ShipInfo/TokensBar").transform)
        {
            keys.Add(icon.gameObject);
        }
        foreach (GameObject icon in keys)
        {
            icon.gameObject.SetActive(false);
            MonoBehaviour.Destroy(icon);
        }

        int columnCounter = 0;
        int rowCounter = 0;
        foreach (var token in thisShip.Tokens.GetAllTokens())
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/PanelToken", typeof(GameObject));
            GameObject tokenPanel = MonoBehaviour.Instantiate(prefab, thisShip.InfoPanel.transform.Find("ShipInfo").Find("TokensBar"));
            tokenPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
            tokenPanel.name = token.Name;
            Tooltips.AddTooltip(tokenPanel, token.Tooltip);
            tokenPanel.transform.Find(token.Name).gameObject.SetActive(true);

            if (token.GetType().BaseType == typeof(Tokens.GenericTargetLockToken))
            {
                tokenPanel.transform.Find(token.Name).Find("Letter").GetComponent<Text>().text = (token as Tokens.GenericTargetLockToken).Letter.ToString();
            }

            tokenPanel.SetActive(true);
            tokenPanel.GetComponent<RectTransform>().localPosition = new Vector3(columnCounter * 37, tokenPanel.GetComponent<RectTransform>().localPosition.y + -37 * rowCounter, tokenPanel.GetComponent<RectTransform>().localPosition.z);
            columnCounter++;
            if (columnCounter == 8)
            {
                rowCounter++;
                columnCounter = 0;
            }
        }

        OrganizeRosters();
    }

    public static void UpdateUpgradesPanel(GenericShip newShip, GameObject newPanel)
    {
        int index = 1;
        foreach (var upgrade in newShip.UpgradeBar.GetUpgradesAll())
        {
            GameObject upgradeNamePanel = newPanel.transform.Find("ShipInfo/UpgradesBar/Upgrade"+index).gameObject;
            upgradeNamePanel.GetComponent<Text>().text = upgrade.Name;
            upgradeNamePanel.SetActive(true);
            index++;
        }
        OrganizeRosters();
    }

    public static void SubscribeUpgradesPanel(GenericShip newShip, GameObject newPanel)
    {
        int index = 1;
        foreach (var upgrade in newShip.UpgradeBar.GetUpgradesAll())
        {
            GameObject upgradeNamePanel = newPanel.transform.Find("ShipInfo/UpgradesBar/Upgrade" + index).gameObject;

            SubscribeSelectionByInfoPanel(upgradeNamePanel);
            Tooltips.AddTooltip(upgradeNamePanel, upgrade.ImageUrl);

            index++;
        }
    }

    public static void DiscardUpgrade(GenericShip host, string upgradeName)
    {
        foreach (Transform upgradeLine in host.InfoPanel.transform.Find("ShipInfo/UpgradesBar").transform)
        {
            if (upgradeLine.GetComponent<Text>().text == upgradeName && upgradeLine.GetComponent<Text>().color != Color.gray)
            {
                upgradeLine.GetComponent<Text>().color = Color.gray;
                return;
            }
        }
    }

    public static void FlipFaceupUpgrade(GenericShip host, string upgradeName)
    {
        foreach (Transform upgradeLine in host.InfoPanel.transform.Find("ShipInfo/UpgradesBar").transform)
        {
            if (upgradeLine.GetComponent<Text>().text == upgradeName && upgradeLine.GetComponent<Text>().color == Color.gray)
            {
                upgradeLine.GetComponent<Text>().color = Color.white;
                return;
            }
        }
    }

    public static void ReplaceUpgrade(GenericShip host, string oldName, string newName, string newImageUrl)
    {
        foreach (Transform upgradeLine in host.InfoPanel.transform.Find("ShipInfo/UpgradesBar").transform)
        {
            if (upgradeLine.GetComponent<Text>().text == oldName)
            {
                upgradeLine.GetComponent<Text>().text = newName;
                upgradeLine.GetComponent<Text>().color = Color.white;
                Tooltips.ReplaceTooltip(upgradeLine.gameObject, newImageUrl);
                return;
            }
        }
    }

    private static void HideAssignedDials()
    {
        foreach (var panel in rosterPlayer1) panel.transform.Find("DialAssigned1").gameObject.SetActive(false);
        foreach (var panel in rosterPlayer2) panel.transform.Find("DialAssigned2").gameObject.SetActive(false);
    }

    public static void RosterAllPanelsHighlightOff()
    {
        foreach (var ship in AllShips)
        {
            RosterPanelHighlightOff(ship.Value);
        }
    }

    private static void RosterPanelHighlightOn(GenericShip ship)
    {
        ship.HighlightCanBeSelectedOn();
        ship.InfoPanel.transform.Find("ShipInfo").GetComponent<Animator>().enabled = true;
    }

    public static void RosterPanelHighlightOff(GenericShip ship)
    {
        ship.InfoPanel.transform.Find("ShipInfo").GetComponent<Animator>().enabled = false;
        ship.InfoPanel.transform.Find("ShipInfo").GetComponent<Image>().color = new Color32(0, 0, 0, 200);
    }

    public static void MarkShip(GenericShip ship, Color color)
    {
        ship.InfoPanel.transform.Find("Mark").GetComponent<Canvas>().enabled = true;
        ship.InfoPanel.transform.Find("Mark").GetComponent<Image>().color = color;
    }

    public static void UnMarkShip(GenericShip ship)
    {
        ship.InfoPanel.transform.Find("Mark").GetComponent<Canvas>().enabled = false;
    }

    public static void UpdateAssignedManeuverDial(GenericShip ship, Movement.GenericMovement maneuver)
    {
        GameObject maneuverDial = ship.InfoPanel.transform.Find("AssignedManeuverDial").gameObject;

        Text maneuverSpeed = maneuverDial.transform.Find("Holder").Find("ManeuverSpeed").GetComponent<Text>();
        maneuverSpeed.text = maneuver.Speed.ToString();
        maneuverSpeed.color = maneuver.GetColor();
        maneuverSpeed.gameObject.SetActive(ship.Owner.IsNeedToShowManeuver(ship));

        Text maneuverBearing = maneuverDial.transform.Find("Holder").Find("ManeuverBearing").GetComponent<Text>();
        maneuverBearing.text = maneuver.GetBearingChar();
        maneuverBearing.color = maneuver.GetColor();
        maneuverBearing.gameObject.SetActive(ship.Owner.IsNeedToShowManeuver(ship));

        maneuverDial.SetActive(true);
    }

    private static bool IsNeedToShowManeuver(GenericShip ship)
    {
        bool result = true;
        if ((ship.Owner.GetType() == typeof(HotacAiPlayer)) && Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase)) return false;
        return result;
    }

    public static void ToggelManeuverVisibility(GenericShip ship, bool isVisible)
    {
        GameObject maneuverDial = ship.InfoPanel.transform.Find("AssignedManeuverDial").gameObject;
        maneuverDial.transform.Find("Holder").Find("ManeuverSpeed").gameObject.SetActive(isVisible);
        maneuverDial.transform.Find("Holder").Find("ManeuverBearing").gameObject.SetActive(isVisible);
    }

    public static void HideAssignedManeuverDial(GenericShip ship)
    {
        GameObject maneuverDial = ship.InfoPanel.transform.Find("AssignedManeuverDial").gameObject;
        maneuverDial.SetActive(false);
    }

    private static void TogglePanelActive(GenericShip ship, bool isActive)
    {
        float colorCode = (isActive) ? 0f : 0.5f;
        ship.InfoPanel.transform.Find("ShipInfo").GetComponent<Image>().color = new Color(colorCode, colorCode, colorCode, (float)(200f / 256f));
    }

    public static void SetPlayerCustomization()
    {
        for (int i = 1; i < 3; i++)
        {
            GenericPlayer player = Roster.GetPlayer(i);
            int playerInfoSlot = (Network.IsNetworkGame && !Network.IsServer) ? Roster.AnotherPlayer(i) : i;
            player.PlayerInfoPanel = GameObject.Find("UI/PlayersPanel/Player" + playerInfoSlot + "Panel");

            player.PlayerInfoPanel.transform.Find("PlayerAvatarImage").GetComponent<AvatarFromUpgrade>().Initialize(Roster.GetPlayer(i).Avatar);
            player.PlayerInfoPanel.transform.Find("PlayerNickName").GetComponent<Text>().text = Roster.GetPlayer(i).NickName;
            player.PlayerInfoPanel.transform.Find("PlayerTitle").GetComponent<Text>().text = Roster.GetPlayer(i).Title;
        }
    }

    public static void HighlightPlayer(PlayerNo playerNo)
    {
        HighlightOfPlayersTurnOff();

        TogglePlayerHighlight(GetPlayer(playerNo), true);
    }

    public static void HighlightOfPlayersTurnOff()
    {
        foreach (var player in Players)
        {
            TogglePlayerHighlight(player, false);
        }
    }

    private static void TogglePlayerHighlight(GenericPlayer player, bool isActive)
    {
        player.PlayerInfoPanel.transform.Find("Highlight").gameObject.SetActive(isActive);
    }

}
