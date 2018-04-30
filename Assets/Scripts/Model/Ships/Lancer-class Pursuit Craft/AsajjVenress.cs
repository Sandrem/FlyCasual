using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using SubPhases;
using Board;
using Abilities;
using System.Linq;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class AsajjVentress : LancerClassPursuitCraft
        {
            public AsajjVentress() : base()
            {
                PilotName = "Asajj Ventress";
                PilotSkill = 6;
                Cost = 37;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new AsajjVentressPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class AsajjVentressPilotAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += TryRegisterAsajjVentressPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= TryRegisterAsajjVentressPilotAbility;
        }

        private void TryRegisterAsajjVentressPilotAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                CheckAssignStress,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostShip.PilotName,
                "Choose a ship inside yout mobile firing arc to assign Stress token to it.",
                HostShip.ImageUrl

            );
        }

        private bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 2) && FilterTargetInMobileFiringArc(ship);
        }

        private int GetAiPriorityOfTarget(GenericShip ship)
        {
            int priority = 50;

            priority += (ship.Tokens.CountTokensByType(typeof(Tokens.StressToken)) * 25);
            priority += (ship.Agility * 5);

            if (ship.CanPerformActionsWhileStressed && ship.CanPerformRedManeuversWhileStressed) priority = 10;

            return priority;
        }

        private bool FilterTargetInMobileFiringArc(GenericShip ship)
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(HostShip, ship);
            return shotInfo.InMobileArc;
        }

        private void CheckAssignStress()
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(HostShip, TargetShip);
            if (shotInfo.InMobileArc && shotInfo.Range >= 1 && shotInfo.Range <= 2)
            {
                Messages.ShowError(HostShip.PilotName + " assigns Stress Token\nto " + TargetShip.PilotName);
                TargetShip.Tokens.AssignToken(new Tokens.StressToken(TargetShip), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                if (!shotInfo.InMobileArc) Messages.ShowError("Target is not inside Mobile Arc");
                else if (shotInfo.Distance >= 3) Messages.ShowError("Target is outside range 2");
            }
        }

    }
}
