using Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Upgrade;
using System.IO;
using Ship;

namespace SquadBuilderNS
{
    public static partial class SquadBuilder
    {
        private const int SHIP_COLUMN_COUNT = 5;
        private const float PILOT_CARD_WIDTH = 300;
        private const float PILOT_CARD_HEIGHT = 418;
        private const float UPGRADE_CARD_WIDTH = 194;
        private const float UPGRADE_CARD_HEIGHT = 300;
        private const float DISTANCE_LARGE = 40;
        private const float DISTANCE_MEDIUM = 20;
        private const float DISTANCE_SMALL = 10;

        private class UpgradeSlotPanel
        {
            public GameObject Panel;
            public Vector2 Size = new Vector2(UPGRADE_CARD_WIDTH, UPGRADE_CARD_HEIGHT);
            public GenericUpgrade Upgrade;
            public UpgradeType SlotType;

            public UpgradeSlotPanel(GenericUpgrade upgrade, UpgradeType slotType, GameObject panel)
            {
                Upgrade = upgrade;
                SlotType = slotType;
                Panel = panel;
            }
        }

        public class ShipWithUpgradesPanel
        {
            public GameObject Panel;
            public Vector2 Size = new Vector2(PILOT_CARD_WIDTH, PILOT_CARD_HEIGHT);
            public SquadBuilderShip Ship;

            public ShipWithUpgradesPanel(SquadBuilderShip ship, GameObject panel)
            {
                Ship = ship;
                Panel = panel;
            }
        }

        private static int availableShipsCounter;
        private static int availablePilotsCounter;
        private static int availableUpgradesCounter;

        private static List<ShipWithUpgradesPanel> ShipWithUpgradesPanels;
        private static ShipWithUpgradesPanel AddShipButtonPanel;

        private static List<UpgradeSlotPanel> UpgradeSlotPanels;

        private static void ShowAvailableShips(Faction faction)
        {
            DestroyChildren(GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);
            availableShipsCounter = 0;

            foreach (ShipRecord ship in AllShips)
            {
                if (ship.Instance.factions.Contains(faction) && !ship.Instance.IsHidden)
                {
                    ShowAvailableShip(ship);
                }
            }
        }

        private static void ShowAvailableShip(ShipRecord ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipPanel", typeof(GameObject));
            GameObject newShipPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);

            ShipPanelSquadBuilder script = newShipPanel.GetComponent<ShipPanelSquadBuilder>();
            script.ImageUrl = GetImageOfIconicPilot(ship);
            script.ShipName = ship.ShipName;

            int row = availableShipsCounter / SHIP_COLUMN_COUNT;
            int column = availableShipsCounter - (row * SHIP_COLUMN_COUNT);

            newShipPanel.transform.localPosition = new Vector3(25 + column * PILOT_CARD_WIDTH + 25 * (column), - (DISTANCE_MEDIUM + row * 184 + DISTANCE_MEDIUM * (row)), 0);

            availableShipsCounter++;
        }

        private static string GetImageOfIconicPilot(ShipRecord ship)
        {
            string image = null;

            if (ship.Instance.IconicPilots != null)
            {
                image = AllPilots.Find(n => n.PilotTypeName == ship.Instance.IconicPilots[CurrentSquadList.SquadFaction].ToString()).Instance.ImageUrl;
            }

            return image;
        }

        private static void ShowAvailablePilots(Faction faction, string shipName)
        {
            availablePilotsCounter = 0;

            ShipRecord shipRecord = AllShips.Find(n => n.ShipName == shipName);
            List<PilotRecord> AllPilotsFiltered = AllPilots.Where(n => n.PilotShip == shipRecord && n.PilotFaction == faction).OrderByDescending(n => n.PilotSkill).ToList();
            int pilotsCount = AllPilotsFiltered.Count;

            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            DestroyChildren(contentTransform);
            contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(pilotsCount*(PILOT_CARD_WIDTH + DISTANCE_MEDIUM) + 2 * DISTANCE_MEDIUM, 0);

            foreach (PilotRecord pilot in AllPilotsFiltered)
            {
                ShowAvailablePilot(pilot);
            }
        }

        private static void DestroyChildren(Transform transformHolder)
        {
            foreach (Transform oldPanel in transformHolder)
            {
                oldPanel.name = "DestructionIsPlanned";
                GameObject.Destroy(oldPanel.gameObject);
            }

            Resources.UnloadUnusedAssets();
        }

        private static void ShowAvailablePilot(PilotRecord pilotRecord)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(newShip, PilotSelectedIsClicked, true);

            int column = availablePilotsCounter;

            newPilotPanel.transform.localPosition = new Vector3(DISTANCE_MEDIUM + (PILOT_CARD_WIDTH + DISTANCE_MEDIUM) * column, PILOT_CARD_HEIGHT/2, 0);

            availablePilotsCounter++;
        }

        public static void PilotSelectedIsClicked(GenericShip ship)
        {
            AddPilotToSquad(ship, CurrentPlayer);
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }

        private static void GenerateShipWithUpgradesPanels()
        {
            ShipWithUpgradesPanels = new List<ShipWithUpgradesPanel>();
            DestroyChildren(GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered/SquadListPanel").transform);

            CreateShipWithUpgradesPanels();

            ShowAddShipPanel();

            OrganizeShipWithUpgradesPanels();
        }

        private static void CreateShipWithUpgradesPanels()
        {
            foreach (SquadBuilderShip ship in CurrentSquadList.GetShips())
            {
                AddShipWithUpgradesPanel(ship);
            }
        }

        private static void AddShipWithUpgradesPanel(SquadBuilderShip ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
            GameObject shipWithUpgradesPanelGO = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered/SquadListPanel").transform);
            ShipWithUpgradesPanel shipWithUpgradesPanel = new ShipWithUpgradesPanel(ship, shipWithUpgradesPanelGO);
            ShipWithUpgradesPanels.Add(shipWithUpgradesPanel);
            ship.Panel = shipWithUpgradesPanel;

            Transform contentTransform = ship.Panel.Panel.transform;
            RectTransform contentRect = contentTransform.GetComponent<RectTransform>();
            int installedUpgradesCount = ship.Instance.UpgradeBar.GetUpgradesAll().Count;
            contentRect.sizeDelta = new Vector2(PILOT_CARD_WIDTH + (UPGRADE_CARD_WIDTH + DISTANCE_SMALL) * installedUpgradesCount, contentRect.sizeDelta.y);

            prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            GameObject pilotPanel = MonoBehaviour.Instantiate(prefab, shipWithUpgradesPanelGO.transform);
            pilotPanel.transform.localPosition = Vector3.zero;

            PilotPanelSquadBuilder script = pilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(ship.Instance, OpenShipInfo);

            ShowUpgradesOfPilot(ship);
        }

        private static void ShowUpgradesOfPilot(SquadBuilderShip ship)
        {
            availableUpgradesCounter = 0;

            foreach (GenericUpgrade upgrade in ship.Instance.UpgradeBar.GetUpgradesAll())
            {
                ShowUpgradeOfPilot(upgrade, ship);
            }
        }

        private static void ShowUpgradeOfPilot(GenericUpgrade upgrade, SquadBuilderShip ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            Transform contentTransform = ship.Panel.Panel.transform;
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            RectTransform contentRect = contentTransform.GetComponent<RectTransform>();
            newUpgradePanel.transform.localPosition = new Vector2(PILOT_CARD_WIDTH + DISTANCE_SMALL + (UPGRADE_CARD_WIDTH + DISTANCE_SMALL) * availableUpgradesCounter, 0);
            ship.Panel.Size = contentRect.sizeDelta;

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.Name, null, upgrade);

            availableUpgradesCounter++;
        }

        private static void ShowAddShipPanel()
        {
            if (GetCurrentSquadCost() < 89)
            {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
                GameObject addShipButtonPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered/SquadListPanel").transform);
                AddShipButtonPanel = new ShipWithUpgradesPanel(null, addShipButtonPanel);

                prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/AddShipButton", typeof(GameObject));
                GameObject addShipButton = MonoBehaviour.Instantiate(prefab, addShipButtonPanel.transform);

                Sprite factionSprite = GameObject.Find("UI/Panels").transform.Find("SelectFactionPanel").Find("Panel").Find("FactionPanels").Find("Button" + CurrentSquadList.SquadFaction.ToString()).GetComponent<Image>().sprite;
                addShipButton.GetComponent<Image>().sprite = factionSprite;

                EventTrigger trigger = addShipButton.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(delegate { OpenSelectShip(); });
                trigger.triggers.Add(entry);
            }
            else
            {
                AddShipButtonPanel = null;
            }                
        }

        private static void OrganizeShipWithUpgradesPanels()
        {
            float allPanelsWidth = 0;
            float defaultWidth = 1366 - 2 * DISTANCE_MEDIUM;

            foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
            {
                allPanelsWidth = allPanelsWidth + panel.Size.x;
            }

            if (ShipWithUpgradesPanels.Count > 1)
            {
                allPanelsWidth += DISTANCE_LARGE * (ShipWithUpgradesPanels.Count - 1);
            }

            if (AddShipButtonPanel != null)
            {
                allPanelsWidth += AddShipButtonPanel.Size.x;
                if (ShipWithUpgradesPanels.Count > 0)
                {
                    allPanelsWidth += DISTANCE_LARGE;
                }
            }

            if (defaultWidth/allPanelsWidth > 0.75f)
            {
                ArrangeShipsWithUpgradesInOneLine(allPanelsWidth);
            }
            else
            {
                ArrangeShipsWithUpgradesInTwoLines(allPanelsWidth);
            }
            
        }

        private static void ArrangeShipsWithUpgradesInOneLine(float allPanelsWidth)
        {
            float defaultWidth = 1366 - 2 * DISTANCE_MEDIUM;
            float offset = 0;

            GameObject centerPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered");
            centerPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(allPanelsWidth, PILOT_CARD_HEIGHT);

            foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
            {
                panel.Panel.transform.localPosition = new Vector2(offset, 0);
                offset += panel.Size.x + DISTANCE_LARGE;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, 0);
            }

            float scale = Mathf.Min(defaultWidth / allPanelsWidth, 1);
            centerPanel.transform.localScale = new Vector2(scale, scale);
        }

        private static void ArrangeShipsWithUpgradesInTwoLines(float allPanelsWidth)
        {
            float defaultHeight = 600;
            float defaultWidth = 1366 - 2 * DISTANCE_MEDIUM;
            float offset = 0;

            Dictionary<ShipWithUpgradesPanel, int> panelsByRow = GetArrangeShipsWithUpgradesIntoRowNumbers();
            float row1width = panelsByRow.Where(n => n.Value == 1).Sum(m => m.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Count(n => n.Value == 1) - 1);
            float row2width = panelsByRow.Where(n => n.Value == 2).Sum(m => m.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Count(n => n.Value == 1) - 1);
            if (AddShipButtonPanel != null) row2width += AddShipButtonPanel.Size.x;
            float maxWidth = Mathf.Max(row1width, row2width);

            GameObject centerPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered");
            centerPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, 2 * PILOT_CARD_HEIGHT + DISTANCE_MEDIUM);

            offset = (maxWidth - row1width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 1))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, 0);
                offset += panel.Key.Size.x + DISTANCE_LARGE;
            }

            offset = (maxWidth - row2width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 2))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, -(PILOT_CARD_HEIGHT + DISTANCE_MEDIUM));
                offset += panel.Key.Size.x + DISTANCE_LARGE;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, -(PILOT_CARD_HEIGHT + DISTANCE_MEDIUM));
            }

            float verticalScale = Mathf.Min(PILOT_CARD_HEIGHT / defaultHeight, 1);

            float firstRowWidth = panelsByRow.Where(n => n.Value == 1).Sum(n => n.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Where(n => n.Value == 1).ToList().Count - 1);
            float secondRowWidth = panelsByRow.Where(n => n.Value == 2).Sum(n => n.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Where(n => n.Value == 2).ToList().Count - 1);

            if (AddShipButtonPanel != null) secondRowWidth += AddShipButtonPanel.Size.x + DISTANCE_LARGE;

            float scaleRow1 = defaultWidth / (firstRowWidth * verticalScale);
            float scaleRow2 = defaultWidth / (secondRowWidth * verticalScale);
            float horizontalScale = Mathf.Min(scaleRow1, scaleRow2, 1);

            float scale = verticalScale * horizontalScale;
            centerPanel.transform.localScale = new Vector2(scale, scale);
        }

        private static Dictionary<ShipWithUpgradesPanel, int> GetArrangeShipsWithUpgradesIntoRowNumbers()
        {
            Dictionary<ShipWithUpgradesPanel, int> result = new Dictionary<ShipWithUpgradesPanel, int>();

            bool isAddShipPanelVisible = AddShipButtonPanel != null;

            ShipWithUpgradesPanel maxSizePanel = ShipWithUpgradesPanels.Find(n => n.Size.x == ShipWithUpgradesPanels.Max(m => m.Size.x));
            result.Add(maxSizePanel, 1);

            float maxWidth = ShipWithUpgradesPanels.Sum(n => n.Size.x) - maxSizePanel.Size.x;
            if (isAddShipPanelVisible) maxWidth += AddShipButtonPanel.Size.x;
            float difference = Mathf.Abs(maxWidth - maxSizePanel.Size.x);

            bool finished = false;
            while (!finished)
            {
                List<ShipWithUpgradesPanel> shipPanelsNotProcessed = ShipWithUpgradesPanels.Where(n => !result.ContainsKey(n)).ToList();
                ShipWithUpgradesPanel minSizePanel = shipPanelsNotProcessed.Find(n => n.Size.x == shipPanelsNotProcessed.Min(m => m.Size.x));

                if (minSizePanel == null) return result;

                float firstRowWidth = result.Where(n => n.Value == 1).Sum(n => n.Key.Size.x) + minSizePanel.Size.x;
                float secondRowWidth = maxWidth - result.Where(n => n.Value == 1).Sum(n => n.Key.Size.x);
                float newDifference = Mathf.Abs(firstRowWidth - secondRowWidth);

                if (newDifference > difference)
                {
                    foreach (ShipWithUpgradesPanel panel in shipPanelsNotProcessed)
                    {
                        if (!result.ContainsKey(panel)) result.Add(panel, 2);
                    }
                    finished = true;
                }
                else
                {
                    result.Add(minSizePanel, 1);
                    difference -= minSizePanel.Size.x;
                    maxWidth -= minSizePanel.Size.x;
                }
            }

            return result;
        }

        private static void UpdateSquadCostForSquadBuilder(int squadCost)
        {
            UpdateSquadCost(squadCost, "SquadBuilderPanel");
        }

        private static void UpdateSquadCostForPilotMenu(int squadCost)
        {
            UpdateSquadCost(squadCost, "ShipSlotsPanel");
        }

        private static void UpdateSquadCostForUpgradesMenu(int squadCost)
        {
            UpdateSquadCost(squadCost, "SelectUpgradePanel");
        }

        private static void UpdateSquadCost(int squadCost, string panelName)
        {
            Text targetText = GameObject.Find("UI/Panels/" + panelName + "/ControlsPanel/SquadCostText").GetComponent<Text>();
            targetText.text = squadCost.ToString() + " / 100";
            targetText.color = (squadCost > 100) ? new Color(1, 0, 0, 200f/255f) : new Color(0, 0, 0, 200f / 255f);
        }

        private static void GenerateShipWithSlotsPanels()
        {
            DestroyChildren(GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/Centered/ShipWithSlotsHolderPanel").transform);

            CreatePilotPanel();
            CreateSlotsPanels();

            OrganizeShipWithSlotsPanels();
        }

        private static void CreatePilotPanel()
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/Centered/ShipWithSlotsHolderPanel/").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);
            newPilotPanel.transform.localPosition = Vector3.zero;

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(CurrentSquadBuilderShip.Instance);
        }

        private static void CreateSlotsPanels()
        {
            UpgradeSlotPanels = new List<UpgradeSlotPanel>();
            foreach (UpgradeSlot slot in CurrentSquadBuilderShip.Instance.UpgradeBar.GetUpgradeSlots())
            {
                //Skip for slots with empty upgrade
                if (!slot.IsEmpty && slot.InstalledUpgrade.GetType() == typeof(UpgradesList.EmptyUpgrade)) continue;

                GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
                Transform contentTransform = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/Centered/ShipWithSlotsHolderPanel/").transform;
                GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

                UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();

                if (slot.InstalledUpgrade == null)
                {
                    script.Initialize("Slot:" + slot.Type.ToString(), slot, null, OpenSelectUpgradeMenu);
                    UpgradeSlotPanels.Add(new UpgradeSlotPanel(null, slot.Type, newUpgradePanel));
                }
                else
                {
                    //script.Initialize(slot.InstalledUpgrade.Name, slot, slot.InstalledUpgrade, RemoveInstalledUpgrade);
                    script.Initialize(slot.InstalledUpgrade.Name, slot, slot.InstalledUpgrade, RemoveUpgradeClicked);
                    UpgradeSlotPanels.Add(new UpgradeSlotPanel(slot.InstalledUpgrade, slot.Type, newUpgradePanel));
                }
            }
        }

        private static void OrganizeShipWithSlotsPanels()
        {
            float defaultWidth = 1366 - 2 * SquadBuilder.DISTANCE_MEDIUM;
            int maxOneRowSize = (UpgradeSlotPanels.Count < 5 ) ? 4 : Mathf.CeilToInt((float)UpgradeSlotPanels.Count / 2f);

            int count = 0;
            float offsetX = PILOT_CARD_WIDTH + 2 * DISTANCE_MEDIUM;
            float offsetY = 0;
            float maxSizeY = PILOT_CARD_HEIGHT;
            float maxSizeX = 0;
            foreach (var upgradeSlotPanel in UpgradeSlotPanels)
            {
                if (count == maxOneRowSize)
                {
                    maxSizeX = offsetX;
                    offsetX = (UpgradeSlotPanels.Count % 2 == 0) ? PILOT_CARD_WIDTH + 2 * DISTANCE_MEDIUM : PILOT_CARD_WIDTH + 2 * DISTANCE_MEDIUM + (UPGRADE_CARD_WIDTH + DISTANCE_MEDIUM) / 2;
                    offsetY = -(UPGRADE_CARD_HEIGHT + DISTANCE_MEDIUM);
                    maxSizeY = 2 * UPGRADE_CARD_HEIGHT + DISTANCE_MEDIUM;
                }
                upgradeSlotPanel.Panel.transform.localPosition = new Vector2(offsetX, offsetY);
                offsetX += upgradeSlotPanel.Size.x + DISTANCE_MEDIUM;
                count++;
            }

            maxSizeX = Mathf.Max(maxSizeX, offsetX);
            Transform centeredHolder = UpgradeSlotPanels.First().Panel.transform.parent.parent;
            centeredHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(maxSizeX, maxSizeY);

            float scale = Mathf.Min(defaultWidth / maxSizeX, 1);
            centeredHolder.transform.localScale = new Vector2(scale, scale);
        }

        private static void ShowAvailableUpgrades(UpgradeSlot slot)
        {
            availableUpgradesCounter = 0;

            List<UpgradeRecord> filteredUpgrades = AllUpgrades.Where(n => n.Instance.hasType(slot.Type) && n.Instance.IsAllowedForShip(CurrentSquadBuilderShip.Instance) && n.Instance.HasEnoughSlotsInShip(CurrentSquadBuilderShip.Instance)).ToList();
            int filteredUpgradesCount = filteredUpgrades.Count;

            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;
            DestroyChildren(contentTransform);
            contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(filteredUpgradesCount * (UPGRADE_CARD_WIDTH + DISTANCE_MEDIUM) + 2 * DISTANCE_MEDIUM, 0);

            foreach (UpgradeRecord upgrade in filteredUpgrades)
            {
                ShowAvailableUpgrade(upgrade);
            }
        }

        private static void ShowAvailableUpgrade(UpgradeRecord upgrade)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            string upgradeType = AllUpgrades.Find(n => n.UpgradeName == upgrade.UpgradeName).UpgradeTypeName;
            GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.UpgradeName, CurrentUpgradeSlot, newUpgrade, SelectUpgradeClicked, true);

            int column = availableUpgradesCounter;

            newUpgradePanel.transform.localPosition = new Vector3(DISTANCE_MEDIUM + (UPGRADE_CARD_WIDTH + DISTANCE_MEDIUM) * column, UPGRADE_CARD_HEIGHT/2, 0);

            availableUpgradesCounter++;
        }

        private static void SelectUpgradeClicked(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            InstallUpgrade(slot, upgrade);

            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }

        private static void RemoveUpgradeClicked(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            RemoveInstalledUpgrade(slot, upgrade);

            // check if its a dual upgrade
            if (upgrade.Types.Count > 1) {
                // find another slot
                int slotsRemoved = 1; // We removed one above (fixes bug #708) TODO: this may not work for multi-type upgrades. Will need to revisit later.
                foreach (UpgradeSlot tempSlot in CurrentSquadBuilderShip.Instance.UpgradeBar.GetUpgradeSlots()){
                    if (slotsRemoved < upgrade.Types.Count && tempSlot != slot && upgrade.hasType (tempSlot.Type)) {
                        slotsRemoved += 1; // Fixes bug #708
                        RemoveInstalledUpgrade (tempSlot, upgrade);
                    }
                }
            }

            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }

        private static void ShowNextButtonFor(PlayerNo playerNo)
        {
            GameObject nextButton = GameObject.Find("UI/Panels/SquadBuilderPanel/ControlsPanel").transform.Find("NextButton").gameObject;
            GameObject startGameButton = GameObject.Find("UI/Panels/SquadBuilderPanel/ControlsPanel").transform.Find("StartGameButton").gameObject;
            if (playerNo == PlayerNo.Player1 && !IsNetworkGame)
            {
                nextButton.SetActive(true);
                startGameButton.SetActive(false);
            }
            else if (playerNo == PlayerNo.Player2 || IsNetworkGame)
            {
                nextButton.SetActive(false);
                startGameButton.SetActive(true);
            }
        }

        public static void OpenImportExportPanel(bool isImport)
        {
            MainMenu.CurrentMainMenu.ChangePanel("ImportExportPanel");

            GameObject.Find("UI/Panels/ImportExportPanel/InputField").GetComponent<InputField>().text = (isImport) ? "" : GetSquadInJson(CurrentPlayer).ToString();
            GameObject.Find("UI/Panels/ImportExportPanel/ControlsPanel/ImportButton").SetActive(isImport);
        }

        private static List<JSONObject> GetSavedSquadsJsons()
        {
            List<JSONObject> savedSquadsJsons = new List<JSONObject>();

            string directoryPath = Application.persistentDataPath + "/SavedSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                string content = File.ReadAllText(filePath);
                JSONObject squadJson = new JSONObject(content);
                squadJson.AddField("filename", new FileInfo(filePath).Name);
                savedSquadsJsons.Add(squadJson);
            }

            return savedSquadsJsons;
        }

        public static void ShowListOfSavedSquadrons(List<JSONObject> squadsJsonsList)
        {
            SetNoSavedSquadronsMessage(squadsJsonsList.Count == 0);

            float FREE_SPACE = 10f;

            Transform contentTransform = GameObject.Find("UI/Panels/BrowseSavedSquadsPanel/Scroll View/Viewport/Content").transform;

            DestroyChildren(contentTransform);

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/SavedSquadronPanel", typeof(GameObject));

            RectTransform contentRectTransform = contentTransform.GetComponent<RectTransform>();
            Vector3 currentPosition = new Vector3(contentRectTransform.sizeDelta.x / 2 + FREE_SPACE, -FREE_SPACE, contentTransform.localPosition.z);

            foreach (var squadList in squadsJsonsList)
            {
                GameObject SquadListRecord;

                SquadListRecord = MonoBehaviour.Instantiate(prefab, contentTransform);
                SquadListRecord.transform.localPosition = currentPosition;

                SquadListRecord.transform.Find("Name").GetComponent<Text>().text = squadList["name"].str;

                Text descriptionText = SquadListRecord.transform.Find("Description").GetComponent<Text>();
                RectTransform descriptionRectTransform = SquadListRecord.transform.Find("Description").GetComponent<RectTransform>();
                descriptionText.text = squadList["description"].str.Replace("\\\"", "\"");
                float descriptionPreferredHeight = descriptionText.preferredHeight;
                descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, descriptionPreferredHeight);

                SquadListRecord.transform.Find("PointsValue").GetComponent<Text>().text = squadList["points"].i.ToString();

                SquadListRecord.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    SquadListRecord.GetComponent<RectTransform>().sizeDelta.x,
                    15 + 50 + 10 + descriptionPreferredHeight + 10 + 50 + 10
                );

                SquadListRecord.name = squadList["filename"].str;

                SquadListRecord.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(delegate { DeleteSavedSquadAndRefresh(SquadListRecord.name); });
                SquadListRecord.transform.Find("LoadButton").GetComponent<Button>().onClick.AddListener(delegate { LoadSavedSquadAndReturn(SquadListRecord.name); });
            }

            OrganizePanels(contentTransform, FREE_SPACE);
        }

        private static void OrganizePanels(Transform contentTransform, float freeSpace)
        {
            float totalHeight = 0;
            foreach (Transform transform in contentTransform)
            {
                if (transform.name != "DestructionIsPlanned")
                {
                    totalHeight += transform.GetComponent<RectTransform>().sizeDelta.y + freeSpace;
                }
            }
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, totalHeight);

            totalHeight = 10;
            foreach (Transform transform in contentTransform)
            {
                if (transform.name != "DestructionIsPlanned")
                {
                    transform.localPosition = new Vector2(10, -totalHeight);
                    totalHeight += transform.GetComponent<RectTransform>().sizeDelta.y + freeSpace;
                }
            }
        }

        private static JSONObject GetSavedSquadJson(string fileName)
        {
            JSONObject squadJson = null;

            string directoryPath = Application.persistentDataPath + "/SavedSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string filePath = directoryPath + "/" + fileName;

            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                squadJson = new JSONObject(content);
            }

            return squadJson;
        }

        private static void SetNoSavedSquadronsMessage(bool isActive)
        {
            GameObject.Find("UI/Panels/BrowseSavedSquadsPanel/NoSavedSquads").SetActive(isActive);
        }

        private static void DeleteSavedSquadFile(string fileName)
        {
            string filePath = Application.persistentDataPath + "/SavedSquadrons/" + fileName;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void ReturnToSquadBuilder()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }

        public static void UpdateSquadName(string panelName)
        {
            GameObject textGO = GameObject.Find("UI/Panels/" + panelName + "/Panel/SquadBuilderTop/SquadNameButton/Text");
            textGO.GetComponent<Text>().text = CurrentSquadList.Name;
            textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(textGO.GetComponent<Text>().preferredWidth, textGO.GetComponent<RectTransform>().sizeDelta.y);
        }

        public static void PrepareSaveSquadronPanel()
        {
            GameObject.Find("UI/Panels/SaveSquadronPanel/Panel/Name/InputField").GetComponent<InputField>().text = CurrentSquadList.Name;
        }

        public static void TrySaveSquadron(Action callback)
        {
            string squadName = GameObject.Find("UI/Panels/SaveSquadronPanel/Panel/Name/InputField").GetComponent<InputField>().text;
            if (squadName == "") squadName = "Unnamed squadron";

            CurrentSquadList.Name = CleanFileName(squadName);

            // check that directory exists, if not create it
            string directoryPath = Application.persistentDataPath + "/SavedSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string filePath = Application.persistentDataPath + "/SavedSquadrons/" + CurrentSquadList.Name;

            if (File.Exists(filePath + ".json"))
            {
                string originalFilePath = filePath;
                int counter = 1;

                while (File.Exists(filePath + ".json"))
                {
                    counter++;
                    filePath = originalFilePath + counter.ToString();
                }
            }

            File.WriteAllText(filePath + ".json", GetSquadInJson(CurrentPlayer).ToString());

            callback();
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static void ShowSkinButtons()
        {
            List<string> availableSkins = GetAvailableShipSkins(CurrentSquadBuilderShip);

            DestroyChildren(GameObject.Find("UI/Panels/ShipSkinsPanel/Panel/Centered").transform);

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipSkinButton", typeof(GameObject));

            float buttonHeight = prefab.GetComponent<RectTransform>().sizeDelta.y;
            int counter = 0;
            foreach (var availableSkin in availableSkins)
            {
                GameObject newButton = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/ShipSkinsPanel/Panel/Centered").transform);
                newButton.transform.localPosition = new Vector2(0, -buttonHeight * counter);

                newButton.GetComponentInChildren<Text>().text = (availableSkin == CurrentSquadBuilderShip.Instance.SkinName) ? "> " + availableSkin + " <" : availableSkin;

                newButton.GetComponent<Button>().onClick.AddListener(delegate { SetSkinForShip(CurrentSquadBuilderShip, availableSkin); ShowSkinButtons(); });

                counter++;
            }

            counter++;

            GameObject backButton = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/ShipSkinsPanel/Panel/Centered").transform);
            backButton.transform.localPosition = new Vector2(0, -buttonHeight * counter);
            backButton.GetComponentInChildren<Text>().text = "Back";
            backButton.GetComponent<Button>().onClick.AddListener(ReturnToSquadBuilder);
        }

        private static void UpdateSkinButton()
        {
            bool hasSkinsSelection = GetAvailableShipSkins(CurrentSquadBuilderShip).Count > 1;
            GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipSlotsTop/TopButtons/SkinsButton").GetComponent<Button>().interactable = hasSkinsSelection;
        }

        // Random AI

        public static void SetRandomAiSquad(Action callback)
        {
            SetPlayerSquadFromImportedJson(GetRandomAiSquad(), CurrentPlayer, callback);
        }

        private static JSONObject GetRandomAiSquad()
        {
            List<JSONObject> savedSquadsJsons = new List<JSONObject>();

            string directoryPath = Application.persistentDataPath + "/RandomAiSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string[] filePaths = Directory.GetFiles(directoryPath);
            if (filePaths.Length == 0)
            {
                CreatePreGeneratedRandomAiSquads();
                filePaths = Directory.GetFiles(directoryPath);
            }

            int randomFileIndex = UnityEngine.Random.Range(0, filePaths.Length);

            string content = File.ReadAllText(filePaths[randomFileIndex]);
            JSONObject squadJson = new JSONObject(content);

            return squadJson;
        }

        private static void CreatePreGeneratedRandomAiSquads()
        {
            string directoryPath = Application.persistentDataPath + "/RandomAiSquadrons";

            foreach (var squadron in PreGeneratedAiSquadrons.Squadrons)
            {
                string filePath = directoryPath + "/" + squadron.Key + ".json";
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, squadron.Value);
                }
            }
        }

    }

}
