using RulesList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tokens;
using RuleSets;

namespace ActionsList
{

    public class JamAction : GenericAction
    {
        public JamAction()
        {
            Name = DiceModificationName = "Jam";
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
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int HasTokenPriority(GenericShip ship)
        {
            if (Edition.Current is SecondEdition && (ship.Tokens.HasToken(typeof(ReinforceAftToken)) || ship.Tokens.HasToken(typeof(ReinforceForeToken)))) return 110;
            if (ship.Tokens.HasToken(typeof(FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(ActionsList.EvadeAction)) || ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(ActionsList.TargetLockAction)) || ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private bool FilterJamTargets(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo) return false;
            if (ship.Tokens.HasToken(typeof(JamToken))) return false;

            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Selection.ThisShip, ship);
            if (distanceInfo.Range <= 1) return true;

            BoardTools.ShotInfo shotInfo = new BoardTools.ShotInfo(Selection.ThisShip, ship, Selection.ThisShip.PrimaryWeapon);
            if (shotInfo.Range <= 2 && shotInfo.InPrimaryArc) return true;

            return false;
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
            targetShip.Tokens.AssignToken(typeof(JamToken), Triggers.FinishTrigger);
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
