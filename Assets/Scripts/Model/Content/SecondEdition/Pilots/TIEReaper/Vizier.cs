﻿using Abilities.SecondEdition;
using ActionsList;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEReaper
    {
        public class Vizier : TIEReaper
        {
            public Vizier() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Vizier\"",
                    2,
                    45,
                    isLimited: true,
                    abilityType: typeof(VizierAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 115
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VizierAbility : GenericAbility
    {
        private bool RestrictedAbilityIsActivated;

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver.GrantedBy == "Ailerons")
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToPerformCoordinate);
            }
        }

        private void AskToPerformCoordinate(object sender, System.EventArgs e)
        {
            RestrictedAbilityIsActivated = true;
            HostShip.OnActionIsPerformed += CheckActionRestriction;
            HostShip.OnMovementStart += ClearRestrictedAbility;

            HostShip.AskPerformFreeAction(
                new CoordinateAction() { HostShip = HostShip },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you fully execute a speed 1 maneuver using your Adaptive Ailerons ship ability, you may perform a Coordinate action. If you do, skip your Perform Action step.",
                HostShip
            );
        }

        private void CheckActionRestriction(GenericAction action)
        {
            if (action is CoordinateAction && RestrictedAbilityIsActivated)
            {
                Messages.ShowInfo("\"Vizier\" skips their Perform Action step");
                HostShip.IsSkipsActionSubPhase = true;
            }
        }

        private void ClearRestrictedAbility(GenericShip ship)
        {
            HostShip.OnMovementStart -= ClearRestrictedAbility;
            HostShip.OnActionIsPerformed -= CheckActionRestriction;

            RestrictedAbilityIsActivated = false;
        }
    }
}