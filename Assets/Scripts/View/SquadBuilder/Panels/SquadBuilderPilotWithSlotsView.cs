using Editions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Upgrade;

namespace SquadBuilderNS
{
    public class UpgradeSlotPanel
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

    public class SquadBuilderPilotWithSlotsView
    {
        private List<UpgradeSlotPanel> UpgradeSlotPanels;
        private SquadBuilderView View { get; }

        public SquadBuilderPilotWithSlotsView(SquadBuilderView view)
        {
            View = view;
        }

        public void GeneratePilotWithSlotsPanels()
        {
            Transform shipWithSlotsTransform = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipWithSlotsHolderPanel").transform;
            SquadBuilderView.DestroyChildren(shipWithSlotsTransform);
            shipWithSlotsTransform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            CreatePilotPanel();
            CreateSlotsPanels();

            OrganizeShipWithSlotsPanels();
        }

        private void CreatePilotPanel()
        {
            PilotPanelSquadBuilder.WaitingToLoad = 0;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipWithSlotsHolderPanel/").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);
            newPilotPanel.transform.localPosition = new Vector3(SquadBuilderView.DISTANCE_MEDIUM, -SquadBuilderView.DISTANCE_MEDIUM, 0);

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(Global.SquadBuilder.CurrentShip.Instance);
        }

        private void CreateSlotsPanels()
        {
            UpgradeSlotPanels = new List<UpgradeSlotPanel>();
            UpgradePanelSquadBuilder.WaitingToLoad = 0;

            List<UpgradeSlot> availableSlots = Global.SquadBuilder.CurrentShip.Instance.UpgradeBar.GetUpgradeSlots().OrderBy(s => s.Type).ToList();

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
                    script.Initialize("Slot:" + slot.Type.ToString(), slot, null, SquadBuilder.Instance.View.OpenSelectUpgradeMenu, compact: true);
                    UpgradeSlotPanels.Add(new UpgradeSlotPanel(null, slot.Type, newUpgradePanel));
                }
                else
                {
                    script.Initialize(slot.InstalledUpgrade.UpgradeInfo.Name, slot, slot.InstalledUpgrade, RemoveUpgradeClicked, compact: true);
                    UpgradeSlotPanels.Add(new UpgradeSlotPanel(slot.InstalledUpgrade, slot.Type, newUpgradePanel));
                }
            }
        }

        private void OrganizeShipWithSlotsPanels()
        {
            int maxOneRowSize = (UpgradeSlotPanels.Count < 5) ? 4 : Mathf.CeilToInt((float)UpgradeSlotPanels.Count / 2f);

            int count = 0;
            float offsetX = (SquadBuilderView.PILOT_CARD_WIDTH + 2 * SquadBuilderView.DISTANCE_MEDIUM);
            float offsetY = -SquadBuilderView.DISTANCE_MEDIUM;
            float maxSizeY = SquadBuilderView.PILOT_CARD_HEIGHT;
            float maxSizeX = 0;
            foreach (var upgradeSlotPanel in UpgradeSlotPanels)
            {
                if (count == maxOneRowSize)
                {
                    offsetX = (UpgradeSlotPanels.Count % 2 == 0) ? SquadBuilderView.PILOT_CARD_WIDTH + 2 * SquadBuilderView.DISTANCE_MEDIUM : SquadBuilderView.PILOT_CARD_WIDTH + 2 * SquadBuilderView.DISTANCE_MEDIUM + (Edition.Current.UpgradeCardCompactSize.x + SquadBuilderView.DISTANCE_MEDIUM) / 2;
                    offsetY = -(Edition.Current.UpgradeCardCompactSize.y + 2 * SquadBuilderView.DISTANCE_MEDIUM);
                    maxSizeY = (2 * Edition.Current.UpgradeCardCompactSize.y + SquadBuilderView.DISTANCE_MEDIUM);
                }
                upgradeSlotPanel.Panel.transform.localPosition = new Vector2(offsetX, offsetY);
                offsetX += upgradeSlotPanel.Size.x + SquadBuilderView.DISTANCE_MEDIUM;

                maxSizeX = Mathf.Max(maxSizeX, offsetX);
                count++;
            }

            Transform centeredHolder = GameObject.Find("UI/Panels/ShipSlotsPanel/Panel/ShipWithSlotsHolderPanel").transform;
            centeredHolder.GetComponent<RectTransform>().sizeDelta = new Vector2(maxSizeX, maxSizeY + 2 * SquadBuilderView.DISTANCE_MEDIUM);
            MainMenu.ScalePanel(centeredHolder, maxScale: 1.25f, twoBorders: true);
        }

        private void RemoveUpgradeClicked(UpgradeSlot slot, GenericUpgrade upgrade)
        {
            View.RemoveInstalledUpgrade(slot, upgrade);

            // check if its a dual upgrade
            if (upgrade.UpgradeInfo.UpgradeTypes.Count > 1)
            {
                List<UpgradeSlot> shipSlots = Global.SquadBuilder.CurrentShip.Instance.UpgradeBar.GetUpgradeSlots();
                foreach (UpgradeType upgradeType in upgrade.UpgradeInfo.UpgradeTypes)
                {
                    UpgradeSlot hiddenSlot = shipSlots.FirstOrDefault(n => n.InstalledUpgrade is UpgradesList.EmptyUpgrade && n.InstalledUpgrade.UpgradeInfo.Name == upgrade.UpgradeInfo.Name);
                    if (hiddenSlot != null) View.RemoveInstalledUpgrade(hiddenSlot, upgrade);
                }
            }

            MainMenu.CurrentMainMenu.ChangePanel("ShipSlotsPanel");
        }
    }
}
