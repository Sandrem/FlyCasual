using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Upgrade;

namespace SquadBuilderNS
{
    public class ShipWithUpgradesPanel
    {
        public GameObject Panel;
        public Vector2 Size = new Vector2(300, 418);
        public SquadBuilderShip Ship;

        public ShipWithUpgradesPanel(SquadBuilderShip ship, GameObject panel)
        {
            Ship = ship;
            Panel = panel;
        }
    }

    static partial class SquadBuilder
    {
        private class UpgradeSlotPanel
        {
            public GameObject Panel;
            public Vector2 Size = new Vector2(194, 300);
            public GenericUpgrade Upgrade;
            public UpgradeType SlotType;

            public UpgradeSlotPanel(GenericUpgrade upgrade, UpgradeType slotType, GameObject panel)
            {
                Upgrade = upgrade;
                SlotType = slotType;
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
                if (ship.Instance.factions.Contains(faction))
                {
                    ShowAvailableShip(ship);
                }
            }
        }

        private static void ShowAvailableShip(ShipRecord ship)
        {
            int columnsCount = 5;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipPanel", typeof(GameObject));
            GameObject newShipPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);

            ShipPanelSquadBuilder script = newShipPanel.GetComponent<ShipPanelSquadBuilder>();
            script.ImageUrl = GetImageOfIconicPilot(ship);
            script.ShipName = ship.ShipName;

            int row = availableShipsCounter / columnsCount;
            int column = availableShipsCounter - (row * columnsCount);

            newShipPanel.transform.localPosition = new Vector3(25 + column * 300 + 25 * (column), - (20 + row * 184 + 20 * (row)), 0);

            availableShipsCounter++;
        }

        private static string GetImageOfIconicPilot(ShipRecord ship)
        {
            string image = null;

            if (ship.Instance.IconicPilot != null)
            {
                image = AllPilots.Find(n => n.PilotName == ship.Instance.IconicPilot).Instance.ImageUrl;
            }

            return image;
        }

        private static void ShowAvailablePilots(Faction faction, string shipName)
        {
            DestroyChildren(GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform);
            availablePilotsCounter = 0;
            ShipRecord shipRecord = AllShips.Find(n => n.ShipName == shipName);

            List<PilotRecord> AllPilotsFiltered = AllPilots.Where(n => n.PilotShip == shipRecord && n.Instance.faction == faction).OrderByDescending(n => n.PilotSkill).ToList();

            foreach (PilotRecord pilot in AllPilotsFiltered)
            {
                ShowAvailablePilot(pilot);
            }
        }

        private static void DestroyChildren(Transform transformHolder)
        {
            foreach (Transform oldPanel in transformHolder)
            {
                GameObject.Destroy(oldPanel.gameObject);
            }
        }

        private static void ShowAvailablePilot(PilotRecord pilotRecord)
        {
            float widthPilotPanel = 300;
            float heightPilotPanel = 418;
            float widthBetweenX = 20;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(pilotRecord.PilotName, CurrentShip, pilotRecord.Instance.ImageUrl, AddPilotToSquadAndReturn);

            int column = availablePilotsCounter;

            newPilotPanel.transform.localPosition = new Vector3(widthBetweenX + (300 + widthBetweenX) * column, heightPilotPanel/2, 0);
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(newPilotPanel.transform.localPosition.x + widthPilotPanel + widthBetweenX, 0);

            availablePilotsCounter++;
        }

        public static void AddPilotToSquadAndReturn(SquadBuilderShip ship, string pilotName, string shipName)
        {
            AddPilotToSquad(pilotName, shipName);
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

            prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            GameObject pilotPanel = MonoBehaviour.Instantiate(prefab, shipWithUpgradesPanelGO.transform);
            pilotPanel.transform.localPosition = Vector3.zero;

            PilotPanelSquadBuilder script = pilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(ship, OpenShipInfo);

            ShowUpgradesOfPilot(ship);
        }

        private static void ShowUpgradesOfPilot(SquadBuilderShip ship)
        {
            foreach (GenericUpgrade upgrade in ship.Instance.UpgradeBar.GetInstalledUpgrades())
            {
                ShowUpgradeOfPilot(upgrade, ship);
            }
        }

        private static void ShowUpgradeOfPilot(GenericUpgrade upgrade, SquadBuilderShip ship)
        {
            float widthBetweenX = 10;
            float widthUpgrade = 194;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            Transform contentTransform = ship.Panel.Panel.transform;
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            RectTransform contentRect = contentTransform.GetComponent<RectTransform>();
            newUpgradePanel.transform.localPosition = new Vector2(contentRect.sizeDelta.x + widthBetweenX, 0);
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x + widthBetweenX + widthUpgrade, contentRect.sizeDelta.y);
            ship.Panel.Size = contentRect.sizeDelta;

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.Name, null, null, null);
        }

        private static void ShowAddShipPanel()
        {
            if (GetSquadCost() < 89)
            {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
                GameObject addShipButtonPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered/SquadListPanel").transform);
                AddShipButtonPanel = new ShipWithUpgradesPanel(null, addShipButtonPanel);

                prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/AddShipButton", typeof(GameObject));
                GameObject addShipButton = MonoBehaviour.Instantiate(prefab, addShipButtonPanel.transform);

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
            float distanceBetweenX = 40;
            float defaultWidth = 1326;

            foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
            {
                allPanelsWidth = allPanelsWidth + panel.Size.x;
            }

            if (ShipWithUpgradesPanels.Count > 2)
            {
                allPanelsWidth += distanceBetweenX * (ShipWithUpgradesPanels.Count - 1);
            }

            if (AddShipButtonPanel != null)
            {
                allPanelsWidth += AddShipButtonPanel.Size.x;
            }
            
            if (ShipWithUpgradesPanels.Count > 0)
            {
                allPanelsWidth += distanceBetweenX;
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
            float defaultWidth = 1326;
            float defaultCardHeight = 418;
            float distanceBetweenX = 40;
            float offset = 0;

            GameObject centerPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered");
            centerPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(allPanelsWidth, defaultCardHeight);

            foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
            {
                panel.Panel.transform.localPosition = new Vector2(offset, 0);
                offset += panel.Size.x + distanceBetweenX;
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
            float defaultCardHeight = 418;
            float distanceBetweenX = 40;
            float distanceBetweenY = 20;
            float offset = 0;

            Dictionary<ShipWithUpgradesPanel, int> panelsByRow = GetArrangeShipsWithUpgradesIntoRowNumbers();
            float row1width = panelsByRow.Where(n => n.Value == 1).Sum(m => m.Key.Size.x) + distanceBetweenX * (panelsByRow.Count(n => n.Value == 1) - 1);
            float row2width = panelsByRow.Where(n => n.Value == 2).Sum(m => m.Key.Size.x) + distanceBetweenX * (panelsByRow.Count(n => n.Value == 1) - 1);
            if (AddShipButtonPanel != null) row2width += AddShipButtonPanel.Size.x;
            float maxWidth = Mathf.Max(row1width, row2width);

            GameObject centerPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/Centered");
            centerPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, 2*defaultCardHeight + distanceBetweenY);

            offset = (maxWidth - row1width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 1))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, 0);
                offset += panel.Key.Size.x + distanceBetweenX;
            }

            offset = (maxWidth - row2width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 2))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, -(defaultCardHeight + distanceBetweenY));
                offset += panel.Key.Size.x + distanceBetweenX;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, -(defaultCardHeight + distanceBetweenY));
            }

            float scale = Mathf.Min(defaultCardHeight / defaultHeight, 1);
            centerPanel.transform.localScale = new Vector2(scale, scale);
        }

        private static Dictionary<ShipWithUpgradesPanel, int> GetArrangeShipsWithUpgradesIntoRowNumbers()
        {
            Dictionary<ShipWithUpgradesPanel, int> result = new Dictionary<ShipWithUpgradesPanel, int>();

            ShipWithUpgradesPanel maxSizePanel = ShipWithUpgradesPanels.Find(n => n.Size.x == ShipWithUpgradesPanels.Max(m => m.Size.x));
            result.Add(maxSizePanel, 1);

            float maxWidth = ShipWithUpgradesPanels.Sum(n => n.Size.x) - maxSizePanel.Size.x;
            if (AddShipButtonPanel != null) maxWidth += AddShipButtonPanel.Size.x;
            float difference = Mathf.Abs(maxWidth - maxSizePanel.Size.x);

            bool finished = false;
            while (!finished)
            {
                ShipWithUpgradesPanel minSizePanel = ShipWithUpgradesPanels.Find(n => n.Size.x == ShipWithUpgradesPanels.Min(m => m.Size.x) && !result.ContainsKey(n));
                float newDifference = Mathf.Abs(result.Sum(n => n.Key.Size.x) + minSizePanel.Size.x - maxWidth + (result.Sum(n => n.Key.Size.x) + minSizePanel.Size.x));
                if (newDifference > difference)
                {
                    foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
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
            targetText.color = (squadCost > 100) ? Color.red : Color.black;
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
            script.Initialize(CurrentSquadBuilderShip);
        }

        private static void CreateSlotsPanels()
        {
            UpgradeSlotPanels = new List<UpgradeSlotPanel>();
            foreach (UpgradeSlot slot in CurrentSquadBuilderShip.Instance.UpgradeBar.GetUpgradeSlots())
            {
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
                    script.Initialize(slot.InstalledUpgrade.Name, slot, null, RemoveInstalledUpgrade);
                    UpgradeSlotPanels.Add(new UpgradeSlotPanel(slot.InstalledUpgrade, slot.Type, newUpgradePanel));
                }
            }
        }

        private static void OrganizeShipWithSlotsPanels()
        {
            float defaultWidth = 1326;
            float pilotCardWidth = 300;
            float pilotCardHeight = 418;
            float upgradeCardHeight = 300;
            float upgradeCardWidth = 194;
            float sizeBetweenX = 20;
            float sizeBetweenY = 20;
            int maxOneRowSize = (UpgradeSlotPanels.Count < 5 ) ? 4 : Mathf.CeilToInt((float)UpgradeSlotPanels.Count / 2f);

            int count = 0;
            float offsetX = pilotCardWidth + 2 * sizeBetweenX;
            float offsetY = 0;
            float maxSizeY = pilotCardHeight;
            float maxSizeX = 0;
            foreach (var upgradeSlotPanel in UpgradeSlotPanels)
            {
                if (count == maxOneRowSize)
                {
                    maxSizeX = offsetX;
                    offsetX = (UpgradeSlotPanels.Count % 2 == 0) ? pilotCardWidth + 2 * sizeBetweenX : pilotCardWidth + 2 * sizeBetweenX + (upgradeCardWidth + sizeBetweenX) / 2;
                    offsetY = -(upgradeCardHeight + sizeBetweenY);
                    maxSizeY = 2 * upgradeCardHeight + sizeBetweenY;
                }
                upgradeSlotPanel.Panel.transform.localPosition = new Vector2(offsetX, offsetY);
                offsetX += upgradeSlotPanel.Size.x + sizeBetweenX;
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
            DestroyChildren(GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform);
            availableUpgradesCounter = 0;

            foreach (UpgradeRecord upgrade in AllUpgrades)
            {
                if (upgrade.Instance.Type == slot.Type)
                {
                    if (upgrade.Instance.IsAllowedForShip(CurrentSquadBuilderShip.Instance))
                    {
                        ShowAvailableUpgrade(upgrade);
                    }
                }
            }
        }

        private static void ShowAvailableUpgrade(UpgradeRecord upgrade)
        {
            float widthBetweenX = 20;
            float widthUpgrade = 194;
            float heightUpgrade = 300;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.UpgradeName, CurrentUpgradeSlot, null, SelectUpgradeClicked);

            int column = availableUpgradesCounter;

            newUpgradePanel.transform.localPosition = new Vector3(widthBetweenX + (widthUpgrade + widthBetweenX) * column, heightUpgrade/2, 0);
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(newUpgradePanel.transform.localPosition.x + widthUpgrade + widthBetweenX, 0);

            availableUpgradesCounter++;
        }

        private static void SelectUpgradeClicked(UpgradeSlot slot, string upgradeName)
        {
            string upgradeType = AllUpgrades.Find(n => n.UpgradeName == upgradeName).UpgradeTypeName;
            GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));
            CurrentUpgradeSlot.PreInstallUpgrade(newUpgrade, CurrentSquadBuilderShip.Instance);

            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }
    }

}
