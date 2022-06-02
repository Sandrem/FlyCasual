using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class Yoda : Eta2Actis
    {
        public Yoda()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Yoda",
                "Grand Master",
                Faction.Republic,
                3,
                5,
                16,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.YodaPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5b/fb/5bfbf23b-29c5-47c9-a6ba-1a2a7ddba319/swz79_yoda.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class YodaPilotAbility : GenericAbility
    {
        private GenericShip TriggeredShip { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnForceTokensAreSpent += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnForceTokensAreSpent -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, ref int count)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);

            if (count > 0
                && HostShip.State.Force > 0
                && HostShip.ShipId != ship.ShipId
                && Tools.IsSameTeam(ship, HostShip)
                && distInfo.Range < 4
            )
            {
                TriggeredShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnForceTokensAreSpent, AskToRestoreCharge);
            }
        }

        private void AskToRestoreCharge(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                RestoreForceCharge,
                descriptionLong: $"Do you want to spend 1 Force to recover used 1 Force of {TriggeredShip.PilotInfo.PilotName}?",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void RestoreForceCharge(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TriggeredShip.State.RestoreForce();
            HostShip.State.SpendForce(1, Triggers.FinishTrigger);
        }
    }
}
