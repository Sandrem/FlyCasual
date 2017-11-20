using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Players;
using GameModes;
using Ship;
using SubPhases;

namespace RulesList
{
    public class DockingRule
    {
        Dictionary<Func<GenericShip>, Func<GenericShip>> dockedShipsPairs = new Dictionary<Func<GenericShip>, Func<GenericShip>>();

        public DockingRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.OnSetupPhaseStart += DockShips;
        }

        public void Dock(Func<GenericShip> host, Func<GenericShip> docked)
        {
            if (host != null && docked != null)
            {
                dockedShipsPairs.Add(docked, host);
            }
        }

        private void DockShips()
        {
            foreach (var dockedShipsPair in dockedShipsPairs)
            {
                GenericShip docked = dockedShipsPair.Key();
                GenericShip host = dockedShipsPair.Value();
                if (host != null && docked != null)
                {
                    Roster.HideShip("ShipId:" + docked.ShipId);
                    host.DockedShips.Add(docked);
                    docked.Model.SetActive(false);

                    docked.CallDocked(host);

                    host.OnMovementFinish += RegisterAskUndock;
                }
            }
        }

        private void RegisterAskUndock(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Undocking decision",
                TriggerType = TriggerTypes.OnShipMovementFinish,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = AskUndock
            });
        }

        private void AskUndock(object sender, EventArgs e)
        {
            GenericShip ship = Selection.ThisShip;

            UndockingDecisionSubPhase newSubphase = (UndockingDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Undocking decision",
                typeof(UndockingDecisionSubPhase),
                Triggers.FinishTrigger
            );

            newSubphase.YesAction = delegate { Undock(ship, ship.DockedShips[0]); };
            newSubphase.Start();            
        }

        private void Undock(GenericShip host, GenericShip docked)
        {
            SetUndockPosition(host, docked);

            Roster.ShowShip(docked);
            host.DockedShips.Remove(docked);
            docked.Model.SetActive(true);

            docked.CallUndocked(host);

            host.OnMovementFinish -= RegisterAskUndock;

            docked.SetAssignedManeuver(new Movement.StraightMovement(2, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.White));
        }

        private void SetUndockPosition(GenericShip host, GenericShip docked)
        {
            docked.SetPosition(host.GetBack());
            docked.SetAngles(host.GetAngles() + new Vector3(0, 180, 0));
        }

    }

}

namespace SubPhases
{

    public class UndockingDecisionSubPhase : DecisionSubPhase
    {
        public Action YesAction;

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Perform undocking?";

            AddDecision("Yes", Undock);
            AddDecision("No", SkipUndock);

            DefaultDecision = "No";

            UI.ShowSkipButton();

            callBack();
        }

        private void Undock(object sender, System.EventArgs e)
        {
            YesAction();
            ConfirmDecision();
        }

        private void SkipUndock(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        public override void SkipButton()
        {
            ConfirmDecision();
        }

    }

}