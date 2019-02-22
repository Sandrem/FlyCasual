﻿using RulesList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Tokens;
using Editions;
using ActionsList;
using Actions;
using SubPhases;

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
            JamTargetSubPhase jamPhase = Phases.StartTemporarySubPhaseNew<JamTargetSubPhase>(
                "Select target to Jam",
                Phases.CurrentSubPhase.CallBack
            );
            jamPhase.HostAction = this;
            jamPhase.Start();
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            if (hasSecondChance)
            {
                UI.ShowSkipButton();
                UI.HighlightSkipButton();
            }
            else
            {
                Phases.GoBack();
            }
        }

    }

}

namespace SubPhases
{

    public class JamTargetSubPhase : SelectShipSubPhase
    {
        public GenericAction HostAction { get; set; }

        public override void Prepare()
        {
            PrepareByParameters(
                SelectJamTarget,
                FilterJamTargets,
                GetAiJamPriority,
                Selection.ThisShip.Owner.PlayerNo,
                false,
                "Jam Action",
                "Select an enemy ship to get a Jam token."
            );
        }

        protected override void CancelShipSelection()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, true);
        }

        public override void SkipButton()
        {
            Rules.Actions.ActionIsFailed(TheShip, HostAction, ActionFailReason.WrongRange, false);
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
            if (Edition.Current is Editions.SecondEdition && (ship.Tokens.HasToken(typeof(ReinforceAftToken)) || ship.Tokens.HasToken(typeof(ReinforceForeToken)))) return 110;
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

            BoardTools.ShotInfo shotInfo = new BoardTools.ShotInfo(Selection.ThisShip, ship, Selection.ThisShip.PrimaryWeapons);
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

        }
    }

}
