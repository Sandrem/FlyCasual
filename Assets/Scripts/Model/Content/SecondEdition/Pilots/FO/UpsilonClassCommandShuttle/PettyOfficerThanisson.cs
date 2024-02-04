using Arcs;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class PettyOfficerThanisson : UpsilonClassCommandShuttle
        {
            public PettyOfficerThanisson() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Petty Officer Thanisson",
                    "Alert Flight Controller",
                    Faction.FirstOrder,
                    1,
                    7,
                    14,
                    isLimited: true,
                    charges: 1,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.PettyOfficerThanissonPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
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

        private void CheckAbility(GenericShip ship, GenericToken token)
        {
            if (Phases.CurrentPhase.GetType() != typeof(MainPhases.ActivationPhase)
                && Phases.CurrentPhase.GetType() != typeof(MainPhases.CombatPhase)
            ) return;

            if (token.GetType() != typeof(StressToken)) return;

            if (HostShip.State.Charges == 0) return;

            if (HostShip.ShipId != ship.ShipId //a ship is never in its own arc
                && HostShip.SectorsInfo.IsShipInSector(ship, ArcType.Front)
                && HostShip.SectorsInfo.RangeToShipBySector(ship, ArcType.Front) <= 2)
            {
                ShipWithStressToken = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, ShowDecision);
            }
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                ShouldUseAbility,
                UseAbility,
                descriptionLong: "Do you want to assign Tractor token to " + ShipWithStressToken.PilotInfo.PilotName + "?",
                imageHolder: HostShip
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
