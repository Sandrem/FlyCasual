using System.Collections.Generic;
using Ship;
using Editions;
using System.Linq;
using System;
using SubPhases;
using ActionsList;

namespace RulesList
{
    public class CollisionRules
    {
        static bool RuleIsInitialized = false;

        public CollisionRules()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
                RuleIsInitialized = true;
            }
            GenericShip.OnMovementFinishGlobal += CheckBumps;
        }

        public void CheckBumps(GenericShip ship)
        {
            if (Selection.ThisShip.ShipsBumpedOnTheEnd.Any(n => Tools.IsSameTeam(Selection.ThisShip, n)))
            {
                Messages.ShowErrorToHuman($"{Selection.ThisShip.PilotInfo.PilotName} collided into another ship. Skipping their action subphase.");
                Selection.ThisShip.IsSkipsActionSubPhase = true;

                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Collision: Roll for damage",
                        TriggerType = TriggerTypes.OnMovementFinish,
                        TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                        EventHandler = StartRollForDamage
                    }
                );
            }
            else if (Selection.ThisShip.ShipsBumpedOnTheEnd.Any(n => Tools.IsAnotherTeam(Selection.ThisShip, n)))
            {
                Messages.ShowErrorToHuman($"{Selection.ThisShip.PilotInfo.PilotName} collided into another ship. Skipping their action subphase.");
                Selection.ThisShip.IsSkipsActionSubPhase = true;

                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Collision: Perform Red Action",
                        TriggerType = TriggerTypes.OnMovementFinish,
                        TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                        EventHandler = AskPerformRedAction
                    }
                );
            }
        }

        private void StartRollForDamage(object sender, EventArgs e)
        {
            Messages.ShowErrorToHuman(Selection.ThisShip.PilotInfo.PilotName + " overlapped friendly ship, rolling for damage");

            OverlappedFriendlyShipDamageCheckSubPhase newPhase = (OverlappedFriendlyShipDamageCheckSubPhase)Phases.StartTemporarySubPhaseNew(
                "Damage from overlapping of friendly ship",
                typeof(OverlappedFriendlyShipDamageCheckSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(OverlappedFriendlyShipDamageCheckSubPhase));
                    Triggers.FinishTrigger();
                });
            newPhase.TheShip = Selection.ThisShip;
            newPhase.Start();
        }

        private void AskPerformRedAction(object sender, EventArgs e)
        {
            if (!Selection.ThisShip.IsStressed)
            {
                List<GenericAction> actionsToPerform = new List<GenericAction>();

                if (Selection.ThisShip.ActionBar.PrintedActions.Any(n => n is FocusAction)) actionsToPerform.Add(new FocusAction() { HostShip = Selection.ThisShip, Color = Actions.ActionColor.Red });
                if (Selection.ThisShip.ActionBar.PrintedActions.Any(n => n is CalculateAction)) actionsToPerform.Add(new CalculateAction() { HostShip = Selection.ThisShip, Color = Actions.ActionColor.Red });

                Selection.ThisShip.AskPerformFreeAction
                (
                    actionsToPerform,
                    Triggers.FinishTrigger,
                    descriptionShort: "Action after overlapping",
                    descriptionLong: "You may perform printed Focus/Coordinate action as red"
                );
            }
            else
            {
                Messages.ShowErrorToHuman($"{Selection.ThisShip.PilotInfo} is stressed, cannot perform action after bump");
                Triggers.FinishTrigger();
            }
        }

        public void AddBump(GenericShip ship1, GenericShip ship2)
        {
            if (!ship1.ShipsBumped.Contains(ship2))
            {
                ship1.ShipsBumped.Add(ship2);
            }

            if (!ship2.ShipsBumped.Contains(ship1))
            {
                ship2.ShipsBumped.Add(ship1);
            }
        }

        public void ClearBumps(GenericShip ship)
        {
            foreach (var bumpedShip in ship.ShipsBumped)
            {
                if (bumpedShip.ShipsBumped.Contains(ship))
                {
                    bumpedShip.ShipsBumped.Remove(ship);
                }
            }
            ship.ShipsBumped = new List<GenericShip>();

            // Clear remotes bumps too
            ship.RemotesOverlapped = new List<Remote.GenericRemote>();
            ship.RemotesMovedThrough = new List<Remote.GenericRemote>();
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            if (!Edition.Current.CanAttackBumpedTarget && 
                Selection.ThisShip.IsBumped && 
                Selection.ThisShip.ShipsBumped.Contains(Selection.AnotherShip) && 
                Selection.AnotherShip.ShipsBumped.Contains(Selection.ThisShip))
            {
                if (!Selection.ThisShip.CanAttackBumpedTarget(Selection.AnotherShip))
                {
                    stringList.Add("You cannot attack the ship that you are touching.");
                    result = false;
                }
            }
        }

    }
}

namespace SubPhases
{

    public class OverlappedFriendlyShipDamageCheckSubPhase : DiceRollCheckSubPhase
    {
        private GenericShip prevActiveShip = Selection.ActiveShip;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
            Selection.ActiveShip = TheShip;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();
            Selection.ActiveShip = prevActiveShip;

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success || CurrentDiceRoll.DiceList[0].Side == DieSide.Crit)
            {
                Messages.ShowErrorToHuman($"{TheShip.PilotInfo.PilotName} is dealt damage");
                TheShip.Damage.TryResolveDamage
                (
                    1,
                    0,
                    new DamageSourceEventArgs
                    {
                        DamageType = DamageTypes.Rules,
                        Source = null
                    },
                    CallBack
                );
            }
            else
            {
                Messages.ShowInfoToHuman("No damage");
                CallBack();
            }
        }
    }
}
