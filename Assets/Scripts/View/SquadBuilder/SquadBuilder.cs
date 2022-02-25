using Players;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;
using Ship;
using Editions;
using System;

namespace SquadBuilderNS
{
    public class SquadBuilderView
    {
        private SquadBuilder Model { get; }

        public static readonly float PILOT_CARD_WIDTH = 300;
        public static readonly float PILOT_CARD_HEIGHT = 418;
        public static readonly float DISTANCE_LARGE = 40;
        public static readonly float DISTANCE_MEDIUM = 25;
        public static readonly float DISTANCE_SMALL = 10;
        public static readonly float SCALE_DEFAULT = 1.25f;

        private SquadBuilderRosterView RosterView { get; }
        private SquadBuilderShipsView ShipsView { get; }
        private SquadBuilderPilotsView PilotsView { get; }
        private SquadBuilderUpgradesView UpgradesView { get; }
        private SquadBuilderPilotWithSlotsView PilotWithSlotsView { get; }
        private SquadBuilderObstaclesView ObstaclesView { get; }
        private SquadBuilderShipInformationView ShipInformationView { get; }
        private SquadBuilderSkinsView SkinsView { get; }
        private SquadsManagement SquadsManagement { get; }

        public SquadBuilderView(SquadBuilder model)
        {
            Model = model;
            Model.View = this;

            RosterView = GameObject.Find("UI/Panels/SquadBuilderPanel/Panel").GetComponent<SquadBuilderRosterView>();
            ShipsView = new SquadBuilderShipsView();
            PilotsView = new SquadBuilderPilotsView();
            UpgradesView = new SquadBuilderUpgradesView();
            PilotWithSlotsView = new SquadBuilderPilotWithSlotsView(this);
            ObstaclesView = new SquadBuilderObstaclesView();
            ShipInformationView = new SquadBuilderShipInformationView();
            SkinsView = new SquadBuilderSkinsView();
            SquadsManagement = new SquadsManagement(this);
        }

        public static void DestroyChildren(Transform transformHolder)
        {
            foreach (Transform oldPanel in transformHolder)
            {
                oldPanel.name = "DestructionIsPlanned";
                GameObject.Destroy(oldPanel.gameObject);
            }

            Resources.UnloadUnusedAssets();
        }

        public void SetRandomAiSquad()
        {
            RandomSquads.SetRandomAiSquad();
        }

        public void UpdateSquadCost(string panelName)
        {
            Text targetText = GameObject.Find("UI/Panels/" + panelName + "/ControlsPanel/SquadCostText").GetComponent<Text>();
            targetText.text = Global.SquadBuilder.CurrentSquad.Points.ToString() + " / " + Edition.Current.MaxPoints;
            targetText.color = (Global.SquadBuilder.CurrentSquad.Points > Edition.Current.MaxPoints) ? new Color(1, 0, 0, 200f/255f) : new Color(0, 0, 0, 200f / 255f);
        }

        public void UpdateLoadoutCost(string panelName, GenericShip ship)
        {
            Text targetText = GameObject.Find("UI/Panels/" + panelName + "/ControlsPanel/SquadCostText").GetComponent<Text>();
            targetText.text = ship.UpgradeBar.GetTotalUsedLoadoutCost() + " / " + (ship.PilotInfo as PilotCardInfo25).LoadoutValue;
            targetText.color = (ship.UpgradeBar.GetTotalUsedLoadoutCost() > (ship.PilotInfo as PilotCardInfo25).LoadoutValue) ? new Color(1, 0, 0, 200f / 255f) : new Color(0, 0, 0, 200f / 255f);
        }

        private void ShowNextButtonFor(PlayerNo playerNo)
        {
            GameObject nextButton = GameObject.Find("UI/Panels/SquadBuilderPanel/ControlsPanel").transform.Find("NextButton").gameObject;
            GameObject startGameButton = GameObject.Find("UI/Panels/SquadBuilderPanel/ControlsPanel").transform.Find("StartGameButton").gameObject;
            if (playerNo == PlayerNo.Player1 && !Global.IsVsNetworkOpponent)
            {
                nextButton.SetActive(true);
                startGameButton.SetActive(false);
            }
            else if (playerNo == PlayerNo.Player2 || Global.IsVsNetworkOpponent)
            {
                nextButton.SetActive(false);
                startGameButton.SetActive(true);
            }
        }

        public void OpenImportExportPanel(bool isImport)
        {
            SquadsManagement.OpenImportExportPanel(isImport);
        }

        public void PrepareSaveSquadronPanel()
        {
            SquadsManagement.PrepareSaveSquadronPanel();
        }

        public void ShowShipInformation()
        {
            ShipInformationView.ShowShipInformation();
        }

        public void ShowSkinsPanel()
        {
            SkinsView.ShowSkinsPanel();
        }

        public void ShowChosenObstaclesPanel()
        {
            ObstaclesView.ShowChosenObstaclesPanel();
        }

        public void ShowBrowseObstaclesPanel()
        {
            ObstaclesView.ShowBrowseObstaclesPanel();
        }

        public void BrowseSavedSquads()
        {
            SquadsManagement.BrowseSavedSquads();
        }

        public static void OrganizePanels(Transform contentTransform, float freeSpace)
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

        public void FilterVisibleUpgrades(string text)
        {
            UpgradesView.FilterVisibleUpgrades(text);
        }

        public void SetDefaultObstacles()
        {
            ObstaclesView.SetDefaultObstacles();
        }

        public void ReturnToSquadBuilder()
        {
            Edition.Current.SquadBuilderIsOpened();
        }

        public void ShowFactionsImages()
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
                string editionName = "SecondEdition";
                Sprite sprite = (Sprite)Resources.Load("Sprites/SquadBuiler/Factions/" + editionName + "/" + imagePanel.name, typeof(Sprite));
                imagePanel.GetComponent<Image>().sprite = sprite;
            }
        }

        private void ShowLoadingContentStub(string panelType)
        {
            GameObject noContentText = GameObject.Find("UI/Panels/Select" + panelType +"Panel").transform.Find("NoContentText")?.gameObject;
            if (noContentText != null) noContentText.SetActive(false);

            GameObject loadingText = GameObject.Find("UI/Panels/Select" + panelType + "Panel").transform.Find("LoadingText").gameObject;
            loadingText.SetActive(true);
        }

        public void ShowShipsFilteredByFaction()
        {
            ShowLoadingContentStub("Ship");
            ShipsView.ShowAvailableShips(Model.CurrentSquad.SquadFaction);
            UpdateSquadCost("SelectShipPanel");
        }

        public void ShowPilotsFilteredByShipAndFaction()
        {
            ShowLoadingContentStub("Pilot");
            PilotsView.ShowAvailablePilots(Model.CurrentSquad.SquadFaction, Global.SquadBuilder.CurrentShipName);
            UpdateSquadCost("SelectPilotPanel");
        }

        public void ShowShipsAndUpgrades()
        {
            UpdateSquadCost("SquadBuilderPanel");
            RosterView.GenerateShipWithUpgradesPanels();
        }

        public void UpdateNextButton()
        {
            ShowNextButtonFor(Model.CurrentPlayer);
        }

        public void ShowPilotWithSlots()
        {
            UpdateLoadoutCost("ShipSlotsPanel", Global.SquadBuilder.CurrentShip.Instance);
            PilotWithSlotsView.GeneratePilotWithSlotsPanels();
        }

        public void OpenSelectShip()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SelectShipPanel");
        }

        public void OpenShipInfo(GenericShip ship)
        {
            Global.SquadBuilder.CurrentShip = Model.CurrentSquad.Ships.Find(n => n.Instance == ship);
            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }

        public void OpenSelectUpgradeMenu(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            Global.SquadBuilder.CurrentUpgradeSlot = slot;
            UpdateLoadoutCost("ShipSlotsPanel", Global.SquadBuilder.CurrentShip.Instance);
            MainMenu.CurrentMainMenu.ChangePanel("SelectUpgradePanel");
        }

        public void RemoveInstalledUpgrade(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            slot.RemovePreInstallUpgrade();
            // check if upgrade is multi-slotted
            ShowPilotWithSlots();
        }

        public void ShowUpgradesList()
        {
            ShowLoadingContentStub("Upgrade");
            UpdateLoadoutCost("SelectUpgradePanel", Global.SquadBuilder.CurrentShip.Instance);
            UpgradesView.ShowAvailableUpgrades(Model.CurrentUpgradeSlot);
        }

        public static void ShowNoContentInfo(string panelTypeName)
        {
            GameObject loadingText = GameObject.Find("UI/Panels/Select" + panelTypeName + "Panel/LoadingText");
            if (loadingText != null) loadingText.SetActive(false);

            GameObject noContentText = GameObject.Find("UI/Panels/Select" + panelTypeName + "Panel").transform.Find("NoContentText").gameObject;
            if (noContentText != null) noContentText.SetActive(true);
        }

        public void LoadSavedSquadAndReturn(JSONObject squadJsonFixed)
        {
            SquadsManagement.LoadSavedSquadAndReturn(squadJsonFixed);
        }
    }
}
