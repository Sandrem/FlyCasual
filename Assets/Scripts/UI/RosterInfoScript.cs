using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//TODO: Store info about ships rosters
public class RosterInfoScript : MonoBehaviour {

    private GameManagerScript Game;

    public GameObject prefabRosterShipPanel;
    public GameObject rosterPanels;

    private List<GameObject> rosterPlayer1 = new List<GameObject>();
    private List<GameObject> rosterPlayer2 = new List<GameObject>();

    // Use this for initialization
    void Start() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Game.PhaseManager.OnCombatPhaseStart += HideAssignedDials;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject CreateRosterInfo(Ship.GenericShip newShip) {

        GameObject newPanel = Instantiate(prefabRosterShipPanel, rosterPanels.transform.Find("TeamPlayer" + PlayerToInt(newShip.PlayerNo)).Find("RosterHolder").transform);

        //Generic info
        newPanel.transform.Find("ShipInfo").Find("ShipPilotSkillText").GetComponent<Text>().text = newShip.PilotSkill.ToString();
        newPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().text = newShip.PilotName;
        newPanel.transform.Find("ShipInfo").Find("ShipTypeText").GetComponent<Text>().text = newShip.Type;
        newPanel.transform.Find("ShipInfo").Find("ShipFirepowerText").GetComponent<Text>().text = newShip.Firepower.ToString();
        newPanel.transform.Find("ShipInfo").Find("ShipAgilityText").GetComponent<Text>().text = newShip.Agility.ToString();
        newPanel.transform.Find("ShipInfo").Find("ShipHullText").GetComponent<Text>().text = newShip.MaxHull.ToString();
        newPanel.transform.Find("ShipInfo").Find("ShipShieldsText").GetComponent<Text>().text = newShip.MaxShields.ToString();

        //Hull and shields
        float panelWidth = 200 - 10;
        float hullAndShield = newShip.MaxHull + newShip.MaxShields;
        float panelWidthNoDividers = panelWidth - (1 * (hullAndShield - 1));
        float damageIndicatorWidth = panelWidthNoDividers / hullAndShield;

        GameObject damageIndicatorBar = newPanel.transform.Find("ShipInfo").Find("DamageBarPanel").gameObject;
        GameObject damageIndicator = damageIndicatorBar.transform.Find("DamageIndicator").gameObject;
        damageIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(damageIndicatorWidth, 10);
        for (int i = 0; i < hullAndShield; i++)
        {
            GameObject newDamageIndicator = Instantiate(damageIndicator, damageIndicatorBar.transform);
            newDamageIndicator.transform.position = damageIndicator.transform.position + new Vector3(i * (damageIndicatorWidth + 1), 0, 0);
            if (i < newShip.Hull) {
                newDamageIndicator.GetComponent<Image>().color = Color.yellow;
                newDamageIndicator.name = "DamageIndicator.Hull." + (i+1).ToString();
            } else
            {
                newDamageIndicator.GetComponent<Image>().color = new Color(0, 202, 255);
                newDamageIndicator.name = "DamageIndicator.Shield." + (i-newShip.Hull+1).ToString();
            }
            newDamageIndicator.SetActive(true);
        }
        Destroy(damageIndicator);

        //Finish
        newPanel.transform.Find("ShipInfo").tag = "ShipId:" + newShip.ShipId.ToString();

        AddToRoster(newShip, newPanel);

        newPanel.transform.Find("ShipInfo").gameObject.AddComponent<EventTrigger>();
        EventTrigger trigger = newPanel.transform.Find("ShipInfo").gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { SelectShipByRosterClick((PointerEventData)data); });
        trigger.triggers.Add(entry);

        newPanel.transform.Find("ShipInfo").gameObject.SetActive(true);

        return newPanel;
    }

    private void AddToRoster(Ship.GenericShip newShip, GameObject newPanel)
    {
        List<GameObject> rosterPlayer = (newShip.PlayerNo == Player.Player1) ? rosterPlayer1 : rosterPlayer2;
        rosterPlayer.Add(newPanel);

        OrganizeRosters();

    }

    private void OrganizeRosters()
    {
        OrganizeRosterPanelSizes();
        OrganizeRosterPositions();
    }

    private int CalculateRosterPanelSize(GameObject panel)
    {
        int panelHeight = 80;

        //Tokens

        bool tokensVisible = false;

        foreach (Transform icon in panel.transform.Find("ShipInfo").Find("TokensBar").transform)
        {
            if (icon.gameObject.activeSelf)
            {
                tokensVisible = true;
                break;
            }
        }

        if (tokensVisible) panelHeight += 40;

        //Upgrades

        float upgradesVisible = 0;

        foreach (Transform icon in panel.transform.Find("ShipInfo").Find("UpgradesBar").transform)
        {
            if (icon.gameObject.activeSelf)
            {
                upgradesVisible++;
                break;
            }
        }

        panelHeight += Mathf.CeilToInt(upgradesVisible/2) * 20;

        return panelHeight;
    }

    private void OrganizeRosterPanelSizes()
    {

        foreach (GameObject panel in rosterPlayer1)
        {
            panel.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta = new Vector2(200, CalculateRosterPanelSize(panel));
        }

        //same
        foreach (GameObject panel in rosterPlayer2)
        {
            panel.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta = new Vector2(200, CalculateRosterPanelSize(panel));
        }

    }

    private void OrganizeRosterPositions()
    {
        Vector3 defaultPosition = rosterPanels.transform.Find("TeamPlayer1").Find("RosterHolder").transform.position + new Vector3(5f, 0f, 0f);

        rosterPlayer1.Sort(delegate (GameObject x, GameObject y)
        {
            return -x.transform.Find("ShipInfo").Find("ShipPilotSkillText").GetComponent<Text>().text.CompareTo(y.transform.Find("ShipInfo").Find("ShipPilotSkillText").GetComponent<Text>().text);
        });

        float offset = 0;
        foreach (var item in rosterPlayer1)
        {
            item.transform.position = defaultPosition + new Vector3(0f, -offset, 0f);
            offset = item.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta.y + 5;
        }

        /// Same for second player
        defaultPosition = rosterPanels.transform.Find("TeamPlayer2").Find("RosterHolder").transform.position + new Vector3(5f, 0f, 0f);

        rosterPlayer2.Sort(delegate (GameObject x, GameObject y)
        {
            return -x.transform.Find("ShipInfo").Find("ShipPilotSkillText").GetComponent<Text>().text.CompareTo(y.transform.Find("ShipInfo").Find("ShipPilotSkillText").GetComponent<Text>().text);
        });

        offset = 0;
        foreach (var item in rosterPlayer2)
        {
            item.transform.position = defaultPosition + new Vector3(0f, -offset, 0f);
            offset = item.transform.Find("ShipInfo").GetComponent<RectTransform>().sizeDelta.y + 5;
        }
    }

    //TODO: rewrite to support TARGETSHIP
    public void SelectShipByRosterClick(PointerEventData data)
    {
        foreach (var item in data.hovered)
        {
            if (item.tag != "Untagged") {
                if (Game.Selection.TryToChangeShip(item.tag)) return;
            }
        }
        Game.UI.HideTemporaryMenus();
    }

    public void UpdateRosterShieldsDamageIndicators(Ship.GenericShip thisShip)
    {
        thisShip.InfoPanel.transform.Find("ShipInfo").Find("ShipShieldsText").GetComponent<Text>().text = thisShip.Shields.ToString();
        foreach (Transform damageIndicator in thisShip.InfoPanel.transform.Find("ShipInfo").Find("DamageBarPanel").transform)
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

    public void UpdateShipStats(Ship.GenericShip thisShip)
    {
        thisShip.InfoPanel.transform.Find("ShipInfo").Find("ShipFirepowerText").GetComponent<Text>().text = thisShip.Firepower.ToString();
        thisShip.InfoPanel.transform.Find("ShipInfo").Find("ShipAgilityText").GetComponent<Text>().text = thisShip.Agility.ToString();
    }

    public void UpdateRosterHullDamageIndicators(Ship.GenericShip thisShip)
    {
        thisShip.InfoPanel.transform.Find("ShipInfo").Find("ShipHullText").GetComponent<Text>().text = thisShip.Hull.ToString();
        foreach (Transform damageIndicator in thisShip.InfoPanel.transform.Find("ShipInfo").Find("DamageBarPanel").transform)
        {
            string[] damageIndicatorData = damageIndicator.name.Split('.');
            string type = damageIndicatorData[1];
            int value = int.Parse(damageIndicatorData[2]);
            if (type == "Hull")
            {
                damageIndicator.gameObject.SetActive(value <= thisShip.Hull);
            }
        }
    }

    public void UpdateTokensIndicator(Ship.GenericShip thisShip)
    {
        foreach (Transform icon in thisShip.InfoPanel.transform.Find("ShipInfo").Find("TokensBar").transform)
        {
            icon.gameObject.SetActive(false);
        }

        float offset = 0;
        foreach (var tokenHolder in thisShip.AssignedTokens)
        {
            GameObject tokenPanel = thisShip.InfoPanel.transform.Find("ShipInfo").Find("TokensBar").Find(tokenHolder.Key.ToString()).gameObject;
            tokenPanel.SetActive(true);
            tokenPanel.GetComponent<RectTransform>().localPosition += new Vector3(offset, 0, 0);
            offset += 32 + 3;
        }

        OrganizeRosters();

    }

    public void UpdateUpgradesPanel(Ship.GenericShip newShip, GameObject newPanel)
    {
        int index = 1;
        foreach (var upgrade in newShip.InstalledUpgrades)
        {
            GameObject upgradeNamePanel = newPanel.transform.Find("ShipInfo").Find("UpgradesBar").Find("Upgrade"+index).gameObject;
            upgradeNamePanel.GetComponent<Text>().text = upgrade.Value.ShortName;
            upgradeNamePanel.SetActive(true);
            index++;
        }
    }

    private void HideAssignedDials()
    {
        foreach (var panel in rosterPlayer1) panel.transform.FindChild("DialAssigned1").gameObject.SetActive(false);
        foreach (var panel in rosterPlayer2) panel.transform.FindChild("DialAssigned2").gameObject.SetActive(false);
    }

    //TODO: move
    public int PlayerToInt(Player playerNo)
    {
        int result = -1;
        if (playerNo == Player.Player1) result = 1;
        if (playerNo == Player.Player2) result = 2;
        return result;
    }

}
