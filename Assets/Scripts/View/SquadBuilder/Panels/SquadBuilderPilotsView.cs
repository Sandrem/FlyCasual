using Editions;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SquadBuilderNS
{
    public class SquadBuilderPilotsView
    {
        public void ShowAvailablePilots(Faction faction, string shipName)
        {
            PilotPanelSquadBuilder.WaitingToLoad = 0;

            ShipRecord shipRecord = Global.SquadBuilder.Database.AllShips.Find(n => n.ShipName == shipName);

            List<PilotRecord> AllPilotsFiltered = Global.SquadBuilder.Database.AllPilots
                .Where(n =>
                    n.Ship == shipRecord
                    && n.PilotFaction == faction
                    && n.Instance.GetType().ToString().Contains(Edition.Current.NameShort)
                    && !n.Instance.IsHiddenSquadbuilderOnly
                    && n.Instance.PilotInfo.GetType() == typeof(Ship.PilotCardInfo25)
                )
                .OrderByDescending(n => n.PilotSkill).
                OrderByDescending(n => n.Instance.PilotInfo.Cost).
                ToList();
            int pilotsCount = AllPilotsFiltered.Count();

            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            SquadBuilderView.DestroyChildren(contentTransform);
            contentTransform.localPosition = new Vector3(0, contentTransform.localPosition.y, contentTransform.localPosition.z);
            contentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(pilotsCount * (SquadBuilderView.PILOT_CARD_WIDTH + SquadBuilderView.DISTANCE_MEDIUM) + 2 * SquadBuilderView.DISTANCE_MEDIUM, 0);

            foreach (PilotRecord pilot in AllPilotsFiltered)
            {
                ShowAvailablePilot(pilot);
            }
        }

        private void ShowAvailablePilot(PilotRecord pilotRecord)
        {
            GameObject prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/PilotPanel", typeof(GameObject));
            Transform contentTransform = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View/Viewport/Content").transform;
            GameObject newPilotPanel = MonoBehaviour.Instantiate(prefab, contentTransform);

            GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
            Edition.Current.AdaptShipToRules(newShip);
            Edition.Current.AdaptPilotToRules(newShip);

            PilotPanelSquadBuilder script = newPilotPanel.GetComponent<PilotPanelSquadBuilder>();
            script.Initialize(newShip, PilotSelectedIsClicked, true);
        }

        public void PilotSelectedIsClicked(GenericShip ship)
        {
            Global.SquadBuilder.CurrentSquad.AddPilotToSquad(ship, isFromUi: true);
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }
    }
}
