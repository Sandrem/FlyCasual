using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;

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

                PilotAbilities.Add(new AbilitiesNamespace.LandoCalrissianAbility());
            }
        }
    }
}

namespace AbilitiesNamespace
{
    public class LandoCalrissianAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnMovementExecuted += CheckLandoCalrissianPilotAbility;
        }

        private void CheckLandoCalrissianPilotAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementExecuted, LandoCalrissianPilotAbility);
            }
        }

        private void LandoCalrissianPilotAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                GrantFreeAction,
                new List<TargetTypes>() {TargetTypes.OtherFriendly},
                new Vector2(1, 1)
            );
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