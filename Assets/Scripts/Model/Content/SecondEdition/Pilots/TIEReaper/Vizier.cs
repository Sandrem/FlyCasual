using Abilities.SecondEdition;
using ActionsList;
using Ship;

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
                    limited: 1,
                    abilityType: typeof(VizierAbility)
                );

                SEImageNumber = 115;
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
            Messages.ShowInfo("\"Vizier\" can perform a Coordinate action");

            RestrictedAbilityIsActivated = true;
            HostShip.OnActionIsPerformed += CheckActionRestriction;
            HostShip.OnMovementStart += ClearRestrictedAbility;

            HostShip.AskPerformFreeAction(new CoordinateAction() { Host = HostShip }, Triggers.FinishTrigger);
        }

        private void CheckActionRestriction(GenericAction action)
        {
            if (action is CoordinateAction && RestrictedAbilityIsActivated)
            {
                Messages.ShowError("\"Vizier\" skips Perform Action step");
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