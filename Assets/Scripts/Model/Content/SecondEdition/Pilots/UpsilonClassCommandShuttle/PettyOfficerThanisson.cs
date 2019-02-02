using Arcs;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class PettyOfficerThanisson : UpsilonClassCommandShuttle
        {
            public PettyOfficerThanisson() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Petty Officer Thanisson",
                    1,
                    60,
                    isLimited: true,
                    charges: 1,
                    regensCharges: true,
                    abilityType: typeof(Abilities.SecondEdition.PettyOfficerThanissonPilotAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/3a2232a5238d8bf5e7538fe1d6003dbc.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PettyOfficerThanissonPilotAbility : GenericAbility
    {
        private GenericShip ShipWithStressToken;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, System.Type tokenType)
        {
            if (Phases.CurrentPhase.GetType() != typeof(MainPhases.ActivationPhase)
                && Phases.CurrentPhase.GetType() != typeof(MainPhases.CombatPhase)
            ) return;

            if (tokenType != typeof(StressToken)) return;

            if (HostShip.State.Charges == 0) return;

            if (HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Front)
                && HostShip.SectorsInfo.RangeToShipBySector(ship, ArcType.Front) <= 2)
            {
                ShipWithStressToken = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, ShowDecision);
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                ShouldUseAbility,
                UseAbility,
                infoText: HostShip.PilotInfo.PilotName + ": Assign Tractor token to " + ShipWithStressToken.PilotInfo.PilotName + "?"
            );
        }

        private bool ShouldUseAbility()
        {
            return ShipWithStressToken.Owner.PlayerNo != HostShip.Owner.PlayerNo;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.SpendCharge();

            TractorBeamToken tractorBeamToken = new TractorBeamToken(ShipWithStressToken, HostShip.Owner);
            ShipWithStressToken.Tokens.AssignToken(tractorBeamToken, Triggers.FinishTrigger);
        }
    }
}
