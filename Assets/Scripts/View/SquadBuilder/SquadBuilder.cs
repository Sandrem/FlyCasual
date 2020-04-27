﻿using Players;
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
using Editions;
using ExtraOptions;
using ExtraOptions.ExtraOptionsList;
using Obstacles;

namespace SquadBuilderNS
{
    public enum FactionSize
    {
        Small4,
        Medium6,
        Medium8,
        Large20
    }

    public static partial class SquadBuilder
    {
        private const float PILOT_CARD_WIDTH = 300;
        private const float PILOT_CARD_HEIGHT = 418;
        private const float DISTANCE_LARGE = 40;
        private const float DISTANCE_MEDIUM = 25;
        private const float DISTANCE_SMALL = 10;
        private const float SCALE_DEFAULT = 1.25f;

        private class UpgradeSlotPanel
        {
            public GameObject Panel;
            public Vector2 Size = new Vector2(Edition.Current.UpgradeCardCompactSize.x, Edition.Current.UpgradeCardCompactSize.y);
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
        
        /// <summary>
        /// Texture cache to improve performance when dealing with textures
        /// </summary>
        public static Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        private static void ShowAvailableShips(Faction faction)
        {
            DestroyChildren(GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);
            availableShipsCounter = 0;

            bool isAnyShipShown = false;
            ShipPanelSquadBuilder.WaitingToLoad = 0;

            foreach (ShipRecord ship in AllShips.OrderBy(s => s.Instance.ShipInfo.ShipName))
            {
                if (ship.Instance.ShipInfo.FactionsAll.Contains(faction) && !ship.Instance.IsHidden && HasPilots(ship, faction))
                {
                    if (ship.Instance.GetType().ToString().Contains(Edition.Current.NameShort))
                    {
                        ShowAvailableShip(ship, faction);
                        isAnyShipShown = true;
                    }
                }
            }

            if (!isAnyShipShown)
            {
                ShowNoContentInfo("Ship");
            }
            else
            {
                ScaleSelectShipPanel(faction);
            }
        }

        private static void ScaleSelectShipPanel(Faction faction)
        {
            string prefabPath = (GetFactionSize(faction) == FactionSize.Large20) ? "Prefabs/SquadBuilder/ShipPanelBig" : "Prefabs/SquadBuilder/ShipPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GridLayoutGroup grid = GameObject.Find("UI/Panels/SelectShipPanel/Panel").GetComponentInChildren<GridLayoutGroup>();
            grid.cellSize = prefab.GetComponent<RectTransform>().sizeDelta;

            switch (GetFactionSize(faction))
            {
                case FactionSize.Large20:
                    grid.constraintCount = 5;
                    break;
                case FactionSize.Medium8:
                    grid.constraintCount = 4;
                    break;
                case FactionSize.Medium6:
                    grid.constraintCount = 3;
                    break;
                case FactionSize.Small4:
                    grid.constraintCount = 2;
                    break;
            }

            float panelWidth = grid.constraintCount * (grid.cellSize.x + 25) + 25;
            int rowsCount = availableShipsCounter / grid.constraintCount;
            if (availableShipsCounter - rowsCount * grid.constraintCount != 0) rowsCount++;
            float panelHeight = (rowsCount) * (grid.cellSize.y + 25) + 25;

            GameObject selechShipPanelGO = GameObject.Find("UI/Panels/SelectShipPanel/Panel");
            selechShipPanelGO.GetComponent<RectTransform>().sizeDelta = new Vector2(panelWidth, panelHeight);
            MainMenu.ScalePanel(selechShipPanelGO.transform);
        }

        private static void ShowNoContentInfo(string panelTypeName)
        {
            GameObject loadingText = GameObject.Find("UI/Panels/Select" + panelTypeName + "Panel/LoadingText");
            if (loadingText != null) loadingText.SetActive(false);

            GameObject noContentText = GameObject.Find("UI/Panels/Select" + panelTypeName + "Panel").transform.Find("NoContentText").gameObject;
            if (noContentText != null) noContentText.SetActive(true);
        }

        private static bool HasPilots(ShipRecord ship, Faction faction)
        {
            return AllPilots.Any(n => n.PilotShip == ship && n.PilotFaction == faction);
        }

        private static void ShowAvailableShip(ShipRecord ship, Faction faction)
        {
            string prefabPath = (GetFactionSize(faction) == FactionSize.Large20) ? "Prefabs/SquadBuilder/ShipPanelBig" : "Prefabs/SquadBuilder/ShipPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject newShipPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SelectShipPanel/Panel").transform);

            ShipPanelSquadBuilder script = newShipPanel.GetComponent<ShipPanelSquadBuilder>();
            script.ImageUrl = GetImageOfIconicPilot(ship);
            script.ShipName = ship.Instance.ShipInfo.ShipName;
            script.FullType = ship.Instance.ShipInfo.ShipName;

            /*int rowsCount = (IsSmallFaction(faction)) ? SHIP_COLUMN_COUNT_SMALLFACTION : SHIP_COLUMN_COUNT;
            int row = availableShipsCounter / rowsCount;
            int column = availableShipsCounter - (row * rowsCount);

            if (IsSmallFaction(faction))
            {
                newShipPanel.transform.localPosition = new Vector3(210 + column * PILOT_CARD_WIDTH * 2 + 50 * (column), -(DISTANCE_MEDIUM + row * 368 + DISTANCE_MEDIUM * 2 * (row)), 0);
            }
            else
            {
                newShipPanel.transform.localPosition = new Vector3(25 + column * PILOT_CARD_WIDTH + 25 * (column), - (DISTANCE_MEDIUM + row * 184 + DISTANCE_MEDIUM * (row)), 0);                
            }*/

            availableShipsCounter++;
        }

        private static FactionSize GetFactionSize(Faction faction)
        {
            switch (faction)
            {
                case Faction.Rebel:
                    return FactionSize.Large20;
                case Faction.Imperial:
                    return FactionSize.Large20;
                case Faction.Scum:
                    return FactionSize.Large20;
                case Faction.Resistance:
                    return FactionSize.Medium8;
                case Faction.FirstOrder:
                    return FactionSize.Medium6;
                case Faction.Republic:
                    return FactionSize.Medium6;
                case Faction.Separatists:
                    return FactionSize.Medium6;
                default:
                    return FactionSize.Large20;
            }
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
            //availablePilotsCounter = 0;
            PilotPanelSquadBuilder.WaitingToLoad = 0;

            ShipRecord shipRecord = AllShips.Find(n => n.ShipName == shipName);

            List<PilotRecord> AllPilotsFiltered = AllPilots
                .Where(n => 
                    n.PilotShip == shipRecord
                    && n.PilotFaction == faction
                    && n.Instance.GetType().ToString().Contains(Edition.Current.NameShort)
                )
                .OrderByDescending(n => n.PilotSkill).
                OrderByDescending(n => n.Instance.PilotInfo.Cost).
                ToList();
            int pilotsCount = AllPilotsFiltered.Count();

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
            Edition.Current.AdaptShipToRules(newShip);
            Edition.Current.AdaptPilotToRules(newShip);

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(newShip, PilotSelectedIsClicked, true);

            //int column = availablePilotsCounter;
            //newPilotPanel.transform.localPosition = new Vector3(DISTANCE_MEDIUM + (PILOT_CARD_WIDTH + DISTANCE_MEDIUM) * column, PILOT_CARD_HEIGHT/2, 0);
            //availablePilotsCounter++;
        }

        public static void PilotSelectedIsClicked(GenericShip ship)
        {
            AddPilotToSquad(ship, CurrentPlayer, isFromUi: true);
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }

        private static void GenerateShipWithUpgradesPanels()
        {
            ShipWithUpgradesPanels = new List<ShipWithUpgradesPanel>();
            DestroyChildren(GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel").transform);

            CreateShipWithUpgradesPanels();

            ShowAddShipPanel();

            OrganizeShipWithUpgradesPanels();
        }

        private static void CreateShipWithUpgradesPanels()
        {
            PilotPanelSquadBuilder.WaitingToLoad = 0;
            foreach (SquadBuilderShip ship in CurrentSquadList.GetShips())
            {
                AddShipWithUpgradesPanel(ship);
            }
        }

        private static void AddShipWithUpgradesPanel(SquadBuilderShip ship)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
            GameObject shipWithUpgradesPanelGO = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel").transform);
            ShipWithUpgradesPanel shipWithUpgradesPanel = new ShipWithUpgradesPanel(ship, shipWithUpgradesPanelGO);
            ShipWithUpgradesPanels.Add(shipWithUpgradesPanel);
            ship.Panel = shipWithUpgradesPanel;

            Transform contentTransform = ship.Panel.Panel.transform;
            RectTransform contentRect = contentTransform.GetComponent<RectTransform>();
            int installedUpgradesCount = ship.Instance.UpgradeBar.GetUpgradesAll().Count;
            contentRect.sizeDelta = new Vector2(PILOT_CARD_WIDTH + (Edition.Current.UpgradeCardCompactSize.x + DISTANCE_SMALL) * installedUpgradesCount, contentRect.sizeDelta.y);

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
            UpgradePanelSquadBuilder.WaitingToLoad = 0;

            foreach (GenericUpgrade upgrade in ship.Instance.UpgradeBar.GetUpgradesAll().OrderBy(s => s.UpgradeInfo.UpgradeTypes[0]))
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
            newUpgradePanel.transform.localPosition = new Vector2(PILOT_CARD_WIDTH + DISTANCE_SMALL + (Edition.Current.UpgradeCardCompactSize.x + DISTANCE_SMALL) * availableUpgradesCounter, 0);
            ship.Panel.Size = contentRect.sizeDelta;

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.UpgradeInfo.Name, null, upgrade, compact: true);

            availableUpgradesCounter++;
        }

        private static void ShowAddShipPanel()
        {
            if
            (
                GetCurrentSquadCost() <= Edition.Current.MaxPoints - Edition.Current.MinShipCost(CurrentSquadList.SquadFaction)
                || ExtraOptionsManager.ExtraOptions[typeof(NoPointsLimitExtraOption)].IsOn
            )
            {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/ShipWithUpgradesPanel", typeof(GameObject));
                GameObject addShipButtonPanel = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel").transform);
                AddShipButtonPanel = new ShipWithUpgradesPanel(null, addShipButtonPanel);

                prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/AddShipButton", typeof(GameObject));
                GameObject addShipButton = MonoBehaviour.Instantiate(prefab, addShipButtonPanel.transform);

                Sprite factionSprite = GameObject.Find("UI/Panels").transform.Find("SelectFactionPanel").Find("Panel").Find("FactionPanels" + Edition.Current.NameShort).Find(CurrentSquadList.SquadFaction.ToString()).GetComponent<Image>().sprite;
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
            float offset = DISTANCE_MEDIUM;

            GameObject shipsWithUpgradesPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel");
            shipsWithUpgradesPanel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            foreach (ShipWithUpgradesPanel panel in ShipWithUpgradesPanels)
            {
                panel.Panel.transform.localPosition = new Vector2(offset, -DISTANCE_MEDIUM);
                offset += panel.Size.x + DISTANCE_LARGE;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, -DISTANCE_MEDIUM);
            }

            shipsWithUpgradesPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(allPanelsWidth + 2*DISTANCE_MEDIUM, PILOT_CARD_HEIGHT + 2*DISTANCE_MEDIUM);
            MainMenu.ScalePanel(shipsWithUpgradesPanel.transform, maxScale: 1.25f, twoBorders: true);
        }

        private static void ArrangeShipsWithUpgradesInTwoLines(float allPanelsWidth)
        {
            float offset = DISTANCE_MEDIUM;

            Dictionary<ShipWithUpgradesPanel, int> panelsByRow = GetArrangeShipsWithUpgradesIntoRowNumbers();
            float row1width = panelsByRow.Where(n => n.Value == 1).Sum(m => m.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Count(n => n.Value == 1) - 1);
            float row2width = panelsByRow.Where(n => n.Value == 2).Sum(m => m.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Count(n => n.Value == 1) - 1);
            if (AddShipButtonPanel != null) row2width += AddShipButtonPanel.Size.x;
            float maxWidth = Mathf.Max(row1width, row2width);

            GameObject shipsWithUpgradesPanel = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadListPanel");
            shipsWithUpgradesPanel.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            offset = (maxWidth - row1width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 1))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, -DISTANCE_MEDIUM);
                offset += panel.Key.Size.x + DISTANCE_LARGE;
            }

            offset = (maxWidth - row2width) / 2;
            foreach (var panel in panelsByRow.Where(n => n.Value == 2))
            {
                panel.Key.Panel.transform.localPosition = new Vector2(offset, -(PILOT_CARD_HEIGHT + 2 * DISTANCE_MEDIUM));
                offset += panel.Key.Size.x + DISTANCE_LARGE;
            }

            if (AddShipButtonPanel != null)
            {
                AddShipButtonPanel.Panel.transform.localPosition = new Vector2(offset, -(PILOT_CARD_HEIGHT + 2 * DISTANCE_MEDIUM));
            }

            /*float firstRowWidth = panelsByRow.Where(n => n.Value == 1).Sum(n => n.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Where(n => n.Value == 1).ToList().Count - 1);
            float secondRowWidth = panelsByRow.Where(n => n.Value == 2).Sum(n => n.Key.Size.x) + DISTANCE_LARGE * (panelsByRow.Where(n => n.Value == 2).ToList().Count - 1);

            if (AddShipButtonPanel != null) secondRowWidth += AddShipButtonPanel.Size.x + DISTANCE_LARGE;*/

            shipsWithUpgradesPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth + 2*DISTANCE_MEDIUM, 2 * PILOT_CARD_HEIGHT + 3 * DISTANCE_MEDIUM);
            MainMenu.ScalePanel(shipsWithUpgradesPanel.transform, maxScale: 1.25f, twoBorders: true);
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

        private static void UpdateSquadCostForShipsMenu(int squadCost)
        {
            UpdateSquadCost(squadCost, "SelectShipPanel");
        }

        private static void UpdateSquadCostForPilotsMenu(int squadCost)
        {
            UpdateSquadCost(squadCost, "SelectPilotPanel");
        }

        private static void UpdateSquadCost(int squadCost, string panelName)
        {
            Text targetText = GameObject.Find("UI/Panels/" + panelName + "/ControlsPanel/SquadCostText").GetComponent<Text>();
            targetText.text = squadCost.ToString() + " / " + Edition.Current.MaxPoints;
            targetText.color = (squadCost > Edition.Current.MaxPoints) ? new Color(1, 0, 0, 200f/255f) : new Color(0, 0, 0, 200f / 255f);
        }

        private static void GenerateShipWithSlotsPanels()
        {
            Transform shipWithSlotsTransform = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipWithSlotsHolderPanel").transform;
            DestroyChildren(shipWithSlotsTransform);
            shipWithSlotsTransform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            CreatePilotPanel();
            CreateSlotsPanels();

            OrganizeShipWithSlotsPanels();
        }

        private static void CreatePilotPanel()
        {
            PilotPanelSquadBuilder.WaitingToLoad = 0;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipWithSlotsHolderPanel/").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);
            newPilotPanel.transform.localPosition = new Vector3(DISTANCE_MEDIUM, -DISTANCE_MEDIUM, 0);

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(CurrentSquadBuilderShip.Instance);
        }

        private static void CreateSlotsPanels()
        {
            UpgradeSlotPanels = new List<UpgradeSlotPanel>();
            UpgradePanelSquadBuilder.WaitingToLoad = 0;

            List<UpgradeSlot> availableSlots = CurrentSquadBuilderShip.Instance.UpgradeBar.GetUpgradeSlots().OrderBy(s => s.Type).ToList();

            foreach (UpgradeSlot slot in availableSlots)
            {
                //Skip for slots with empty upgrade
                if (!slot.IsEmpty && slot.InstalledUpgrade.GetType() == typeof(UpgradesList.EmptyUpgrade)) continue;

                GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
                Transform contentTransform = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipWithSlotsHolderPanel/").transform;
                GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

                UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();

                if (slot.InstalledUpgrade == null)
                {
                    script.Initialize("Slot:" + slot.Type.ToString(), slot, null, OpenSelectUpgradeMenu, compact: true);
                    UpgradeSlotPanels.Add(new UpgradeSlotPanel(null, slot.Type, newUpgradePanel));
                }
                else
                {
                    script.Initialize(slot.InstalledUpgrade.UpgradeInfo.Name, slot, slot.InstalledUpgrade, RemoveUpgradeClicked, compact: true);
                    UpgradeSlotPanels.Add(new UpgradeSlotPanel(slot.InstalledUpgrade, slot.Type, newUpgradePanel));
                }
            }
        }

        private static void OrganizeShipWithSlotsPanels()
        {
            int maxOneRowSize = (UpgradeSlotPanels.Count < 5 ) ? 4 : Mathf.CeilToInt((float)UpgradeSlotPanels.Count / 2f);

            int count = 0;
            float offsetX = (PILOT_CARD_WIDTH + 2 * DISTANCE_MEDIUM);
            float offsetY = -DISTANCE_MEDIUM;
            float maxSizeY = PILOT_CARD_HEIGHT;
            float maxSizeX = 0;
            foreach (var upgradeSlotPanel in UpgradeSlotPanels)
            {
                if (count == maxOneRowSize)
                {
                    offsetX = (UpgradeSlotPanels.Count % 2 == 0) ? PILOT_CARD_WIDTH + 2 * DISTANCE_MEDIUM : PILOT_CARD_WIDTH + 2 * DISTANCE_MEDIUM + (Edition.Current.UpgradeCardCompactSize.x + DISTANCE_MEDIUM) / 2;
                    offsetY = -(Edition.Current.UpgradeCardCompactSize.y + 2*DISTANCE_MEDIUM);
                    maxSizeY = (2 * Edition.Current.UpgradeCardCompactSize.y + DISTANCE_MEDIUM);
                }
                upgradeSlotPanel.Panel.transform.localPosition = new Vector2(offsetX, offsetY);
                offsetX += upgradeSlotPanel.Size.x + DISTANCE_MEDIUM;

                maxSizeX = Mathf.Max(maxSizeX, offsetX);
                count++;
            }

            Transform centeredHolder = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipWithSlotsHolderPanel").transform;
            centeredHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(maxSizeX, maxSizeY + 2*DISTANCE_MEDIUM);
            MainMenu.ScalePanel(centeredHolder, maxScale: 1.25f, twoBorders: true);
        }

        private static void ShowAvailableUpgrades(UpgradeSlot slot)
        {
            availableUpgradesCounter = 0;
            UpgradePanelSquadBuilder.WaitingToLoad = 0;

            List<UpgradeRecord> filteredUpgrades = null;

            if (slot.Type != UpgradeType.Omni)
            {
                filteredUpgrades = AllUpgrades.Where(n =>
                     n.Instance.HasType(slot.Type)
                     && n.Instance.UpgradeInfo.Restrictions.IsAllowedForShip(CurrentSquadBuilderShip.Instance)
                     && n.Instance.IsAllowedForShip(CurrentSquadBuilderShip.Instance)
                     && n.Instance.HasEnoughSlotsInShip(CurrentSquadBuilderShip.Instance)
                     && ShipDoesntHaveUpgradeWithSameName(CurrentSquadBuilderShip.Instance, n.Instance)
                ).ToList();
            }
            else
            {
                filteredUpgrades = AllUpgrades;
            }

            int filteredUpgradesCount = filteredUpgrades.Count();

            //Clear search text
            GameObject.Find("UI/Panels/SelectUpgradePanel/TopPanel/InputField").GetComponent<InputField>().text = "";

            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;
            DestroyChildren(contentTransform);

            if (filteredUpgradesCount > 0)
            {
                contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);
                contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(filteredUpgradesCount * (Edition.Current.UpgradeCardSize.x * 1.5f + DISTANCE_MEDIUM) + 2 * DISTANCE_MEDIUM, 0);

                foreach (UpgradeRecord upgrade in filteredUpgrades)
                {
                    if (upgrade.Instance is IVariableCost && Edition.Current is SecondEdition) (upgrade.Instance as IVariableCost).UpdateCost(CurrentSquadBuilderShip.Instance);
                }

                filteredUpgrades = filteredUpgrades.OrderBy(n => n.Instance.UpgradeInfo.Cost).ToList();

                foreach (UpgradeRecord upgrade in filteredUpgrades)
                {
                    ShowAvailableUpgrade(upgrade);
                }

                GridLayoutGroup grid = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").GetComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(Edition.Current.UpgradeCardSize.x * 1.5f, Edition.Current.UpgradeCardSize.y * 1.5f);
            }
            else
            {
                ShowNoContentInfo("Upgrade");
            }
        }

        private static bool ShipDoesntHaveUpgradeWithSameName(GenericShip ship, GenericUpgrade upgrade)
        {
            return !ship.UpgradeBar.GetUpgradesAll().Any(n => n.UpgradeInfo.Name == upgrade.UpgradeInfo.Name);
        }

        public static void FilterVisibleUpgrades(string text)
        {
            int upgradesCount = 0;
            foreach (Transform transform in GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform)
            {
                if (text == "")
                {
                    transform.gameObject.SetActive(true);
                    upgradesCount++;
                }
                else
                {
                    if (transform.gameObject.name.ToLower().Contains(text))
                    {
                        transform.gameObject.SetActive(true);
                        upgradesCount++;
                    }
                    else
                    {
                        transform.gameObject.SetActive(false);
                    }
                }
            }

            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;

            if (upgradesCount > 0)
            {
                contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);
                contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(upgradesCount * (Edition.Current.UpgradeCardSize.x * 1.5f + DISTANCE_MEDIUM) + 2 * DISTANCE_MEDIUM, 0);
            }
        }

        private static void ShowAvailableUpgrade(UpgradeRecord upgrade)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/UpgradePanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newUpgradePanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            string upgradeType = AllUpgrades.Find(n => n.UpgradeNameCanonical == upgrade.UpgradeNameCanonical && n.UpgradeType == upgrade.UpgradeType).UpgradeTypeName;
            GenericUpgrade newUpgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));
            if (newUpgrade is IVariableCost && Edition.Current is SecondEdition) (newUpgrade as IVariableCost).UpdateCost(CurrentSquadBuilderShip.Instance);
            Edition.Current.AdaptUpgradeToRules(newUpgrade);

            UpgradePanelSquadBuilder script = newUpgradePanel.GetComponent<UpgradePanelSquadBuilder>();
            script.Initialize(upgrade.UpgradeName, CurrentUpgradeSlot, newUpgrade, SelectUpgradeClicked, true);
            newUpgradePanel.name = upgrade.UpgradeName;

            //int column = availableUpgradesCounter;
            //newUpgradePanel.transform.localPosition = new Vector3(DISTANCE_MEDIUM + (Edition.Current.UpgradeCardSize.x + DISTANCE_MEDIUM) * column, Edition.Current.UpgradeCardSize.y / 2, 0);

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
            if (upgrade.UpgradeInfo.UpgradeTypes.Count > 1)
            {
                List<UpgradeSlot> shipSlots = CurrentSquadBuilderShip.Instance.UpgradeBar.GetUpgradeSlots();
                foreach (UpgradeType upgradeType in upgrade.UpgradeInfo.UpgradeTypes)
                {
                    UpgradeSlot hiddenSlot = shipSlots.FirstOrDefault(n => n.InstalledUpgrade is UpgradesList.EmptyUpgrade && n.InstalledUpgrade.UpgradeInfo.Name == upgrade.UpgradeInfo.Name);
                    if (hiddenSlot != null) RemoveInstalledUpgrade(hiddenSlot, upgrade);
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

            GameObject.Find("UI/Panels/ImportExportPanel/Content/InputField").GetComponent<InputField>().text = (isImport) ? "" : GetSquadInJson(CurrentPlayer).ToString();
            GameObject.Find("UI/Panels/ImportExportPanel/BottomPanel/ImportButton").SetActive(isImport);
        }

        private static List<JSONObject> GetSavedSquadsJsons()
        {
            List<JSONObject> savedSquadsJsons = new List<JSONObject>();

            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/" + Edition.Current.PathToSavedSquadrons;
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            foreach (var filePath in Directory.GetFiles(directoryPath))
            {
                string content = File.ReadAllText(filePath);
                JSONObject squadJson = new JSONObject(content);

                if (CurrentSquadList.SquadFaction == Faction.None || Edition.Current.XwsToFaction(squadJson["faction"].str) == CurrentSquadList.SquadFaction)
                {
                    squadJson.AddField("filename", new FileInfo(filePath).Name);
                    savedSquadsJsons.Add(squadJson);
                }
            }

            return savedSquadsJsons;
        }

        public static void ShowListOfSavedSquadrons(List<JSONObject> squadsJsonsList)
        {
            SetNoSavedSquadronsMessage(squadsJsonsList.Count == 0);

            float FREE_SPACE = 25;

            Transform contentTransform = GameObject.Find("UI/Panels/BrowseSavedSquadsPanel/Scroll View/Viewport/Content").transform;

            DestroyChildren(contentTransform);

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/SavedSquadronPanel", typeof(GameObject));

            //RectTransform contentRectTransform = contentTransform.GetComponent<RectTransform>();
            //Vector3 currentPosition = new Vector3(0, -FREE_SPACE, contentTransform.localPosition.z);

            foreach (var squadList in squadsJsonsList)
            {
                GameObject SquadListRecord;

                SquadListRecord = MonoBehaviour.Instantiate(prefab, contentTransform);

                SquadListRecord.transform.Find("Name").GetComponent<Text>().text = squadList["name"].str;

                Text descriptionText = SquadListRecord.transform.Find("Description").GetComponent<Text>();
                RectTransform descriptionRectTransform = SquadListRecord.transform.Find("Description").GetComponent<RectTransform>();
                if (squadList.HasField("description"))
                {
                    descriptionText.text = squadList["description"].str.Replace("\\\"", "\"");
                }
                else
                {
                    descriptionText.text = "No description";
                }
                
                float descriptionPreferredHeight = descriptionText.preferredHeight;
                descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, descriptionPreferredHeight);

                SquadListRecord.transform.Find("PointsValue").GetComponent<Text>().text = squadList["points"].i.ToString();

                SquadListRecord.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    SquadListRecord.GetComponent<RectTransform>().sizeDelta.x,
                    15 + 70 + 10 + descriptionPreferredHeight + 10 + 55 + 10
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
            RectTransform contRect = contentTransform.GetComponent<RectTransform>();
            contRect.sizeDelta = new Vector2(contRect.sizeDelta.x, totalHeight + 25);

            totalHeight = 25;
            foreach (Transform transform in contentTransform)
            {
                if (transform.name != "DestructionIsPlanned")
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, -totalHeight);
                    totalHeight += transform.GetComponent<RectTransform>().sizeDelta.y + freeSpace;
                }
            }
        }

        private static JSONObject GetSavedSquadJson(string fileName)
        {
            JSONObject squadJson = null;

            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/" + Edition.Current.PathToSavedSquadrons;
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
            string filePath = Application.persistentDataPath + "/" + Edition.Current.Name + "/SavedSquadrons/" + fileName;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void ReturnToSquadBuilder()
        {
            Edition.Current.SquadBuilderIsOpened();
        }

        public static void UpdateSquadName(string panelName)
        {
            /*GameObject textGO = GameObject.Find("UI/Panels/" + panelName + "/TopPanel/SquadNameButton/Text");
            textGO.GetComponent<Text>().text = CurrentSquadList.Name;
            textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(textGO.GetComponent<Text>().preferredWidth, textGO.GetComponent<RectTransform>().sizeDelta.y);*/
        }

        public static void PrepareSaveSquadronPanel()
        {
            GameObject.Find("UI/Panels/SaveSquadronPanel/Panel/Name/InputField").GetComponent<InputField>().text = CurrentSquadList.Name;
        }

        public static void SaveSquadronToFile(SquadList squadList, string squadName, Action callback)
        {
            squadList.Name = CleanFileName(squadName);

            // check that directory exists, if not create it
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/SavedSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string filePath = directoryPath + "/" + squadList.Name + ".json";

            if (File.Exists(filePath)) File.Delete(filePath);

            File.WriteAllText(filePath, GetSquadInJson(squadList.PlayerNo).ToString());

            callback();
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        // Random AI

        public static void SetRandomAiSquad(Action callback)
        {
            string filename = "";
            var json = GetRandomAiSquad(out filename);
            SetPlayerSquadFromImportedJson(
                filename,
                json,
                CurrentPlayer,
                delegate {
                    SetAiType(Options.AiType);
                    callback();
                });
        }

        private static JSONObject GetRandomAiSquad(out string filename)
        {
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/RandomAiSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            CreatePreGeneratedRandomAiSquads();

            string[] filePaths = Directory.GetFiles(directoryPath);
            int randomFileIndex = UnityEngine.Random.Range(0, filePaths.Length);

            string content = File.ReadAllText(filePaths[randomFileIndex]);
            JSONObject squadJson = new JSONObject(content);

            filename = Path.GetFileName(filePaths[randomFileIndex]);

            return squadJson;
        }

        // NOT USED
        private static bool IsGenerationOfSquadsRequired()
        {
            return true;
        }

        private static void CreatePreGeneratedRandomAiSquads()
        {
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/RandomAiSquadrons";

            foreach (var squadron in Edition.Current.PreGeneratedAiSquadrons)
            {
                string filePath = directoryPath + "/" + squadron.Key + ".json";
                File.WriteAllText(filePath, squadron.Value);
            }
        }

        public static void ShowFactionsImages()
        {
            List<string> panelNames = new List<string>() { "FactionPanelsFirstEdition", "FactionPanelsSecondEdition" };
            foreach (string panelName in panelNames)
            {
                GameObject.Find("UI/Panels/SelectFactionPanel/Panel").transform.Find(panelName).gameObject.SetActive(false);
            }

            GameObject panelGO = GameObject.Find("UI/Panels/SelectFactionPanel/Panel").transform.Find("FactionPanels" + Edition.Current.NameShort).gameObject;
            MainMenu.ScalePanel(panelGO.transform, maxScale: 1.25f);
            panelGO.SetActive(true);

            foreach (Transform imagePanel in panelGO.transform)
            {
                string editionName = (Edition.Current is FirstEdition) ? "FirstEdition" : "SecondEdition";
                Sprite sprite = (Sprite)Resources.Load("Sprites/SquadBuiler/Factions/" + editionName + "/" + imagePanel.name, typeof(Sprite));
                imagePanel.GetComponent<Image>().sprite = sprite;
            }
        }

        public static void CheckAiButtonVisibility()
        {
            bool isAi = GetSquadList(CurrentPlayer).PlayerType.IsSubclassOf(typeof(GenericAiPlayer));
            // ALTERNATIVE AI IS DISABLED
            // GameObject.Find("UI/Panels/SquadBuilderPanel/Panel/SquadBuilderTop").transform.Find("AIButton").gameObject.SetActive(isAi);

            if (isAi)
            {
                SquadBuilder.SetAiType(Options.AiType);
            }
            
        }

        public static void ShowShipInformation()
        {
            GenericShip ship = (MainMenu.CurrentMainMenu.PreviousPanelName == "SelectPilotPanel") ? AllShips.Find(n => n.ShipName == CurrentShip).Instance : CurrentSquadBuilderShip.Instance;

            Text shipNameText = GameObject.Find("UI/Panels/ShipInfoPanel/Content/TopPanel/ShipTypeText").GetComponent<Text>();
            shipNameText.text = ship.ShipInfo.ShipName;

            Text sizeText = GameObject.Find("UI/Panels/ShipInfoPanel/Content/TopPanel/ShipSizeText").GetComponent<Text>();
            switch (ship.ShipInfo.BaseSize)
            {
                case BaseSize.Small:
                    sizeText.text = "Small Ship";
                    break;
                case BaseSize.Medium:
                    sizeText.text = "Medium Ship";
                    break;
                case BaseSize.Large:
                    sizeText.text = "Large Ship";
                    break;
                default:
                    break;
            }

            Transform parentTransform = GameObject.Find("UI/Panels").transform
                .Find("ShipInfoPanel")
                .Find("Content")
                .Find("CenterPanel");

            GameObject oldDial = parentTransform.Find("ManeuversDialView")?.gameObject;
            if (oldDial != null) GameObject.Destroy(oldDial);

            GameObject dial = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/ManeuversDial/ManeuversDialView"), parentTransform);
            dial.name = "ManeuversDialView";
            dial.GetComponent<ManeuversDialView>().Initialize(ship.DialInfo.PrintedDial, isDisabled: true);
        }

        private static void ShowLoadingContentStub(string panelType)
        {
            GameObject noContentText = GameObject.Find("UI/Panels/Select" + panelType +"Panel").transform.Find("NoContentText")?.gameObject;
            if (noContentText != null) noContentText.SetActive(false);

            GameObject loadingText = GameObject.Find("UI/Panels/Select" + panelType + "Panel").transform.Find("LoadingText").gameObject;
            loadingText.SetActive(true);
        }

        public static void ShowSkinsPanel()
        {
            string prefabPath = "Prefabs/SquadBuilder/SkinSelectionPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject contentGO = GameObject.Find("UI/Panels/SkinsPanel/Content/Scroll View/Viewport/Content").gameObject;

            int shipsCount = CurrentSquadList.GetShips().Count;
            contentGO.GetComponent<RectTransform>().sizeDelta = new Vector2(shipsCount * 600 + ((shipsCount + 1) * 20), 0);

            DestroyChildren(contentGO.transform);
            DestroyChildren(GameObject.Find("PreviewsHolder").transform);
            int i = 0;
            foreach (SquadBuilderShip ship in CurrentSquadList.GetShips())
            {
                GameObject newSkinPanel = GameObject.Instantiate(prefab, contentGO.transform);
                newSkinPanel.GetComponent<SkinSelectionPanelScript>().Initialize(ship.Instance, i++);
            }
        }

        public static void ShowChosenObstaclesPanel()
        {
            string prefabPath = "Prefabs/SquadBuilder/ObstacleViewPanel";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject contentGO = GameObject.Find("UI/Panels/ChosenObstaclesPanel/Content/ObstaclesPanel").gameObject;

            DestroyChildren(contentGO.transform);
            DestroyChildren(GameObject.Find("PreviewsHolder").transform);

            for (int i = 0; i < 3; i++)
            {
                GameObject newObstacleViewPanel = GameObject.Instantiate(prefab, contentGO.transform);
                GenericObstacle obstacle = CurrentSquadList.ChosenObstacles[i];
                newObstacleViewPanel.GetComponent<ObstacleViewPanelScript>().Initialize(
                    obstacle,
                    i+1,
                    delegate {
                        SquadBuilder.CurrentObstacle = obstacle;
                        MainMenu.CurrentMainMenu.ChangePanel("BrowseObstaclesPanel");
                    }
                );
            }

            MainMenu.ScalePanel(contentGO.transform);
        }

        public static void ShowBrowseObstaclesPanel()
        {
            string prefabPath = "Prefabs/SquadBuilder/ObstacleViewPanelSmall";
            GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
            GameObject contentGO = GameObject.Find("UI/Panels/BrowseObstaclesPanel/Content").gameObject;

            DestroyChildren(contentGO.transform);
            DestroyChildren(GameObject.Find("PreviewsHolder").transform);

            for (int i = 0; i < ObstaclesManager.Instance.AllPossibleObstacles.Count; i++)
            {
                GameObject newObstacleViewPanel = GameObject.Instantiate(prefab, contentGO.transform);
                GenericObstacle obstacle = ObstaclesManager.Instance.AllPossibleObstacles[i];
                newObstacleViewPanel.GetComponent<ObstacleViewPanelScript>().Initialize(
                    obstacle,
                    i,
                    delegate {
                        int index = CurrentSquadList.ChosenObstacles.IndexOf(SquadBuilder.CurrentObstacle);
                        CurrentSquadList.ChosenObstacles[index] = obstacle;
                        MainMenu.CurrentMainMenu.ChangePanel("ChosenObstaclesPanel");
                    }
                );
                if (CurrentSquadList.ChosenObstacles.Contains(obstacle) && obstacle != CurrentObstacle)
                {
                    newObstacleViewPanel.GetComponent<Button>().interactable = false;
                }
            }

        }

    }

    /// <summary>
    /// The WebRequest and texture scaling can be very expensive, so cache the textures for future use (navigating items in SquadBuilder)
    /// </summary>
    public static class TextureCache
    {
        
    }
}
