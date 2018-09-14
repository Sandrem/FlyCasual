﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using ActionsList;
using RuleSets;

namespace Ship
{
    namespace YT1300
    {
        public class LandoCalrissian : YT1300, ISecondEditionPilot
        {
            public LandoCalrissian() : base()
            {
                PilotName = "Lando Calrissian";
                PilotSkill = 7;
                Cost = 44;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.LandoCalrissianAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 92;

                PilotAbilities.RemoveAll(ability => ability is Abilities.LandoCalrissianAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.LandoCalrissianAbilitySE());

                SEImageNumber = 70;
            }
        }
    }
}

namespace Abilities
{
    public class LandoCalrissianAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementExecuted += CheckLandoCalrissianPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementExecuted -= CheckLandoCalrissianPilotAbility;
        }

        protected virtual void CheckLandoCalrissianPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementExecuted, LandoCalrissianPilotAbility);
            }
        }

        protected void LandoCalrissianPilotAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                GrantFreeAction,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostShip.PilotName,
                "Choose another ship.\nIt may perform free action shown in its action bar.",
                HostShip.ImageUrl
            );
        }

        protected virtual bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 1);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(EvadeAction)) && !ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(TargetLockAction)) && !ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private void GrantFreeAction()
        {
            Selection.ThisShip = TargetShip;

            RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, SelectShipSubPhase.FinishSelection);
        }

        protected virtual void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = TargetShip.GetAvailableActions();
            List<GenericAction> actionBarActions = actions.Where(n => n.IsInActionBar).ToList();

            TargetShip.AskPerformFreeAction(
                actionBarActions,
                delegate {
                    Selection.ThisShip = HostShip;
                    Phases.CurrentSubPhase.Resume();
                    Triggers.FinishTrigger();
                });
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandoCalrissianAbilitySE : LandoCalrissianAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckLandoCalrissianPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckLandoCalrissianPilotAbility;
        }

        protected override void CheckLandoCalrissianPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, LandoCalrissianPilotAbility);
            }
        }

        protected override bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.This, TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }

        protected override void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = TargetShip.GetAvailableActions();

            TargetShip.AskPerformFreeAction(
                actions,
                delegate {
                    Selection.ThisShip = HostShip;
                    Phases.CurrentSubPhase.Resume();
                    Triggers.FinishTrigger();
                });
        }
    }
}