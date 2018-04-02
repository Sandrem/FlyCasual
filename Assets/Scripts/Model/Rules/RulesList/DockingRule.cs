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

        public void Initialize()
        {
            dockedShipsPairs = new Dictionary<Func<GenericShip>, Func<GenericShip>>();
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
                DockShip(dockedShipsPair.Key(), dockedShipsPair.Value());
            }
        }

        private void DockShip(GenericShip docked, GenericShip host)
        {
            if (host != null && docked != null)
            {
                Roster.DockShip("ShipId:" + docked.ShipId);
                host.DockedShips.Add(docked);
                docked.Host = host;
                docked.Model.SetActive(false);
                host.ToggleDockedModel(docked, true);

                docked.CallDocked(host);

                host.OnMovementFinish += RegisterAskUndock;
                host.OnShipIsDestroyed += CheckForcedUndocking;
            }
        }

        private void RegisterAskUndock(GenericShip ship)
        {
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

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

        private void Undock(GenericShip host, GenericShip docked, bool isForced = false)
        {
            SetUndockPosition(host, docked);

            Roster.UndockShip(docked);
            host.DockedShips.Remove(docked);
            host.ToggleDockedModel(docked, false);
            docked.Model.SetActive(true);

            docked.CallUndocked(host);

            host.OnMovementFinish -= RegisterAskUndock;
            host.OnShipIsDestroyed -= CheckForcedUndocking;

            if (!isForced)
            {
                AskAssignManeuver(host, docked);
            }
            else
            {
                docked.Tokens.AssignToken(new Tokens.WeaponsDisabledToken(docked), delegate{
                    DealFacedownDamageCard(docked, delegate{
                        AskAssignManeuver(host, docked);
                    });
                });
            }
        }

        private void CheckForcedUndocking(GenericShip host, bool isFled)
        {
            if (!isFled)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Deploy docked ship",
                        TriggerType = TriggerTypes.OnShipIsDestroyed,
                        TriggerOwner = host.Owner.PlayerNo,
                        EventHandler = delegate { Undock(host, host.DockedShips[0], true); }
                    }
                );
            }
        }

        // TODO: Create "Deal facedown card method in ship.Damage"

        private void DealFacedownDamageCard(GenericShip dockedShip, Action callback)
        {
            DamageDecks.GetDamageDeck(dockedShip.Owner.PlayerNo).DrawDamageCard(
                false,
                delegate { DealDrawnCard(dockedShip, callback); },
                new DamageSourceEventArgs
                {
                    DamageType = DamageTypes.Rules,
                    Source = null
                }
            );
        }

        private void DealDrawnCard(GenericShip ship, Action callBack)
        {
            ship.Damage.DealDrawnCard(callBack);
        }

        private void AskAssignManeuver(GenericShip host, GenericShip docked)
        {
            Selection.ChangeActiveShip("ShipId:" + docked.ShipId);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Assign undocking maneuver",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = docked.Owner.PlayerNo,
                EventHandler = AskChangeManeuver
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, RegisterPerformManeuver);
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            DirectionsMenu.Show(GameMode.CurrentGameMode.AssignManeuver);
        }

        private void RegisterPerformManeuver()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Undocking Execution",
                TriggerType = TriggerTypes.OnManeuver,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = PerformManeuver
            });

            Triggers.ResolveTriggers(
                TriggerTypes.OnManeuver,
                AfterUndockingManeuverIsFinished
            );
        }

        private void PerformManeuver(object sender, EventArgs e)
        {
            Selection.ThisShip.IsManeuverPerformed = true;
            Roster.AllShipsHighlightOff();

            Selection.ThisShip.ObstaclesHit = new List<Collider>();
            Selection.ThisShip.MinesHit = new List<GameObject>();

            Selection.ThisShip.AssignedManeuver.Perform();
        }

        private void AfterUndockingManeuverIsFinished()
        {
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Action after Undocking",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeAction,
                    EventHandler = PerformFreeAction
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnFreeAction, AfterUndockingFinished);
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            Selection.ThisShip.GenerateAvailableActionsList();
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();
            Selection.ThisShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

        private void AfterUndockingFinished()
        {
            if (!Selection.ThisShip.Host.IsDestroyed)
            {
                Selection.ChangeActiveShip("ShipId:" + Selection.ThisShip.Host.ShipId);
            }
            else
            {
                Selection.ThisShip = Selection.ThisShip.Host;
            }

            Triggers.FinishTrigger();
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
            InfoText = "Deploy docked ship?";

            AddDecision("Yes", Undock);
            AddDecision("No", SkipUndock);

            DefaultDecisionName = "No";

            UI.ShowSkipButton();

            callBack();
        }

        private void Undock(object sender, System.EventArgs e)
        {
            ConfirmDecisionNoCallback();
            YesAction();
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