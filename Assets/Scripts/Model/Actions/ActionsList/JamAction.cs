using RulesList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tokens;

namespace ActionsList
{

    public class JamAction : GenericAction
    {
        public JamAction()
        {
            Name = EffectName = "Jam";
        }

        public override void ActionTake()
        {
            var jamPhase = Phases.StartTemporarySubPhaseNew(
                "Select target to Jam",
                typeof(SubPhases.JamTargetSubPhase),
                Phases.CurrentSubPhase.CallBack
            );
            jamPhase.Start();
        }

    }

}

namespace SubPhases
{

    public class JamTargetSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            PrepareByParameters(
                SelectJamTarget,
                FilterJamTargets,
                GetAiJamPriority,
                Selection.ThisShip.Owner.PlayerNo,
                true,
                "Jam Action",
                "Select an enemy ship to get a Jam token."
            );
        }

        private int GetAiJamPriority(GenericShip ship)
        {
            int result = 0;

            result += HasTokenPriority(ship);
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private int HasTokenPriority(GenericShip ship)
        {
            if (ship.Tokens.HasToken(typeof(Tokens.FocusToken))) return 100;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.EvadeAction)) || ship.Tokens.HasToken(typeof(Tokens.EvadeToken))) return 50;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.TargetLockAction)) || ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private bool FilterJamTargets(GenericShip ship)
        {
            BoardTools.ShipDistanceInfo distanceInfo = new BoardTools.ShipDistanceInfo(Selection.ThisShip, ship);
            return ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo && distanceInfo.Range >= 1 && distanceInfo.Range <= 2;
        }

        private void SelectJamTarget()
        {
            Selection.ThisShip.CallJamTargetIsSelected(TargetShip, PerformJamEffect);
        }

        private void PerformJamEffect()
        {
            var jammingShip = Selection.ThisShip;
            var targetShip = TargetShip;
                        
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Jam",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnTokenIsAssigned,
                    EventHandler = (s,e)=>AssignJamToken(targetShip)
                }
            );

            MovementTemplates.ReturnRangeRuler();

            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, delegate {
                Selection.ThisShip = jammingShip;
                Phases.FinishSubPhase(typeof(JamTargetSubPhase));
                CallBack();
            });
        }

        private void AssignJamToken(GenericShip targetShip)
        {
            targetShip.Tokens.AssignToken(new JamToken(targetShip), Triggers.FinishTrigger);
        }

        public override void RevertSubPhase()
        {
            Selection.ThisShip.RemoveAlreadyExecutedAction(typeof(ActionsList.JamAction));

            Phases.FinishSubPhase(this.GetType());
            Phases.CurrentSubPhase.Resume();
            UpdateHelpInfo();
        }

        public override void SkipButton()
        {
            RevertSubPhase();
        }
    }

}
