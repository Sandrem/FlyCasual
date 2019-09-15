using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Players;
using GameModes;
using Ship;
using SubPhases;
using Tokens;
using Movement;
using Bombs;

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
            Phases.Events.OnGameStart += DockShips;
        }

        // OLD
        public void Dock(Func<GenericShip> host, Func<GenericShip> docked)
        {
            if (host != null && docked != null)
            {
                dockedShipsPairs.Add(docked, host);
            }
        }

        public void Dock(GenericShip hostShip, GenericShip dockedShip)
        {
            if (hostShip != null && dockedShip != null)
            {
                PerformDocking(hostShip, dockedShip);
            }
        }

        private void DockShips()
        {
            foreach (var dockedShipsPair in dockedShipsPairs)
            {
                PerformDockingOld(dockedShipsPair.Key(), dockedShipsPair.Value());
            }
        }

        //old
        private void PerformDockingOld(GenericShip docked, GenericShip host)
        {
            if (host != null && docked != null)
            {
                Roster.DockShip("ShipId:" + docked.ShipId);
                host.DockedShips.Add(docked);
                docked.DockingHost = host;
                docked.Model.SetActive(false);
                host.ToggleDockedModel(docked, true);

                docked.CallDocked(host);
                host.CallAnotherShipDocked(docked);

                host.OnMovementFinish += RegisterAskUndockFE;
                host.OnShipIsDestroyed += CheckForcedUndocking;
            }
        }

        private void PerformDocking(GenericShip hostShip, GenericShip dockedShip)
        {
            Roster.DockShip("ShipId:" + dockedShip.ShipId);
            hostShip.DockedShips.Add(dockedShip);
            dockedShip.DockingHost = hostShip;
            dockedShip.Model.SetActive(false);
            hostShip.ToggleDockedModel(dockedShip, true);

            dockedShip.CallDocked(hostShip);
            hostShip.CallAnotherShipDocked(dockedShip);

            hostShip.OnShipIsDestroyed += CheckForcedUndocking;

            // OLD
            if (Editions.Edition.Current is Editions.SecondEdition)
            {
                hostShip.OnCheckSystemsAbilityActivation += CheckUndockAvailability;
                hostShip.OnSystemsAbilityActivation += RegisterAskUndockSE;
            }
            else
            {
                hostShip.OnMovementFinish += RegisterAskUndockFE;
            }
            
        }

        private void CheckUndockAvailability(GenericShip ship, ref bool flag)
        {
            if (ship.CheckCanReleaseDockedShipRegular()) flag = true;
        }

        private void RegisterAskUndockSE(GenericShip ship)
        {
            if (ship.CheckCanReleaseDockedShipRegular())
            {
                RegisterAskUndock(ship, TriggerTypes.OnSystemsAbilityActivation);
            }
        }

        private void RegisterAskUndockFE(GenericShip ship)
        {
            RegisterAskUndock(ship, TriggerTypes.OnMovementFinish);
        }

        private void RegisterAskUndock(GenericShip ship, TriggerTypes timing)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            bool canUndock = ship.CheckCanReleaseDockedShipRegular();
            if (canUndock)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Undocking decision",
                    TriggerType = timing,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = AskUndock
                });
            }
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

        private void Undock(GenericShip hostShip, GenericShip dockedShip, bool isEmergencyDeploy = false)
        {
            UndockingDirectionDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<UndockingDirectionDecisionSubphase>(
                "Select direction of undocking",
                delegate { ContinueUndocking(hostShip, dockedShip, isEmergencyDeploy); }
            );

            subphase.DescriptionShort = "Select direction of deployment";
            subphase.DecisionOwner = hostShip.Owner;

            List<Direction> allDirections = new List<Direction>() { Direction.Top, Direction.Bottom }
                .Where(n => hostShip.FilterUndockDirection(n))
                .ToList();

            foreach (Direction direction in allDirections)
            {
                subphase.AddDecision(
                    (direction == Direction.Top) ? "Front" : "Rear",
                    delegate { SetUndockPosition(hostShip, dockedShip, direction); },
                    isCentered: true
                );
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private void ContinueUndocking(GenericShip hostShip, GenericShip dockedShip, bool isEmergencyDeploy)
        {
            Roster.UndockShip(dockedShip);
            hostShip.DockedShips.Remove(dockedShip);
            hostShip.ToggleDockedModel(dockedShip, false);
            dockedShip.Model.SetActive(true);

            dockedShip.CallUndocked(hostShip);
            hostShip.CallAnotherShipUndocked(dockedShip);

            if (Editions.Edition.Current is Editions.SecondEdition)
            {
                hostShip.OnSystemsAbilityActivation -= RegisterAskUndockSE;
                hostShip.OnCheckSystemsAbilityActivation -= CheckUndockAvailability;
            }
            else
            {
                hostShip.OnMovementFinish -= RegisterAskUndockFE;
            }

            hostShip.OnShipIsDestroyed -= CheckForcedUndocking;

            if (!isEmergencyDeploy)
            {
                AskAssignManeuver(hostShip, dockedShip);
            }
            else
            {
                if (Editions.Edition.Current is Editions.FirstEdition)
                {
                    dockedShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), delegate
                    {
                        DealFacedownDamageCard(dockedShip, delegate
                        {
                            AskAssignManeuver(hostShip, dockedShip, true);
                        });
                    });
                }
                else {
                    dockedShip.Damage.TryResolveDamage(
                        0, 
                        new DamageSourceEventArgs()
                        {
                            Source = null,
                            DamageType = DamageTypes.Rules
                        }, 
                        delegate
                        {
                            AskAssignManeuver(hostShip, dockedShip, true);
                        }, 
                        1
                        );
                }
            }
        }

        private void SetUndockPosition(GenericShip hostShip, GenericShip dockedShip, Direction direction)
        {
            switch (direction)
            {
                case Direction.Top:
                    SetUndockPositionForward(hostShip, dockedShip);
                    break;
                case Direction.Bottom:
                    SetUndockPositionRear(hostShip, dockedShip);
                    break;
                default:
                    break;
            }

            DecisionSubPhase.ConfirmDecision();            
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

        private void AskAssignManeuver(GenericShip host, GenericShip docked, bool isEmergencyDeploy = false)
        {
            Selection.ChangeActiveShip("ShipId:" + docked.ShipId);

            if (Editions.Edition.Current is Editions.SecondEdition)
            {
                DirectionsMenu.Show(GameMode.CurrentGameMode.AssignManeuver, delegate { RegisterPerformManeuver(isEmergencyDeploy); }, FilterOnlyForward);
            }
            else
            {
                DirectionsMenu.Show(GameMode.CurrentGameMode.AssignManeuver, delegate { RegisterPerformManeuver(isEmergencyDeploy); });
            }
        }

        private bool FilterOnlyForward(string maneuverCode)
        {
            bool result = true;
            GenericMovement maneuver = ShipMovementScript.MovementFromString(maneuverCode);

            if (maneuver.Bearing == ManeuverBearing.Stationary
                || maneuver.Bearing == ManeuverBearing.ReverseStraight
                || maneuver.Bearing == ManeuverBearing.ReverseBank)
            {
                result = false;
            }

            return result;
        }

        private void RegisterPerformManeuver(bool isEmergencyDeploy)
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
                delegate { AfterUndockingManeuverIsFinished(isEmergencyDeploy); }
            );
        }

        private void PerformManeuver(object sender, EventArgs e)
        {
            Selection.ThisShip.IsManeuverPerformed = true;
            Roster.AllShipsHighlightOff();

            Selection.ThisShip.IsHitObstacles = false;
            Selection.ThisShip.MinesHit = new List<GenericDeviceGameObject>();

            Selection.ThisShip.AssignedManeuver.Perform();
        }

        private void AfterUndockingManeuverIsFinished(bool isEmergencyDeploy)
        {
            if (!(Selection.ThisShip.IsDestroyed || (isEmergencyDeploy && Editions.Edition.Current is Editions.SecondEdition)))
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
            }

            Triggers.ResolveTriggers(
                TriggerTypes.OnFreeAction,
                delegate {
                    Selection.ThisShip.CallUndockingFinish(Selection.ThisShip.DockingHost, AfterUndockingFinished);
                }
            );
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            Selection.ThisShip.AskPerformFreeAction(
                actions,
                Triggers.FinishTrigger,
                "Action after deployment"
            );
        }

        private void AfterUndockingFinished()
        {
            if (!Selection.ThisShip.DockingHost.IsDestroyed)
            {
                Selection.ChangeActiveShip("ShipId:" + Selection.ThisShip.DockingHost.ShipId);
            }
            else
            {
                Selection.ThisShip = Selection.ThisShip.DockingHost;
            }

            Triggers.FinishTrigger();
        }

        private void SetUndockPositionRear(GenericShip host, GenericShip docked)
        {
            docked.SetPosition(host.GetBack());
            docked.SetAngles(host.GetAngles() + new Vector3(0, 180, 0));
        }

        private void SetUndockPositionForward(GenericShip host, GenericShip docked)
        {
            docked.SetPosition(host.GetPosition());
            docked.SetAngles(host.GetAngles());
        }

        private class UndockingDirectionDecisionSubphase : DecisionSubPhase { }

    }

}

namespace SubPhases
{

    public class UndockingDecisionSubPhase : DecisionSubPhase
    {
        public Action YesAction;

        public override void PrepareDecision(System.Action callBack)
        {
            DescriptionShort = "Do you want to deploy the docked ship?";

            AddDecision("Yes", Undock);
            AddDecision("No", SkipUndock);

            DefaultDecisionName = "No";

            ShowSkipButton = true;

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