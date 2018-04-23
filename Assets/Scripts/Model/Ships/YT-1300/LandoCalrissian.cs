using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;
using System;
using System.Linq;

namespace Ship
{
    namespace YT1300
    {
        public class LandoCalrissian : YT1300
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

        private void CheckLandoCalrissianPilotAbility(GenericShip ship)
        {
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementExecuted, LandoCalrissianPilotAbility);
            }
        }

        private void LandoCalrissianPilotAbility(object sender, System.EventArgs e)
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

        private bool FilterAbilityTargets(GenericShip ship)
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
            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) return 100;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.EvadeAction)) && !ship.Tokens.HasToken(typeof(Tokens.EvadeToken))) return 50;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.TargetLockAction)) && !ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private void GrantFreeAction()
        {
            Selection.ThisShip = TargetShip;

            RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, SelectShipSubPhase.FinishSelection);
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            TargetShip.AskPerformFreeAction(
                TargetShip.GetAvailablePrintedActionsList(),
                delegate {
                    Selection.ThisShip = HostShip;
                    Phases.CurrentSubPhase.Resume();
                    Triggers.FinishTrigger();
                });
        }
    }
}