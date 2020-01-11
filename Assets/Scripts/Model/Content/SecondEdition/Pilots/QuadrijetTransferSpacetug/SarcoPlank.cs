﻿using Ship;
using SubPhases;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class SarcoPlank : QuadrijetTransferSpacetug
        {
            public SarcoPlank() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sarco Plank",
                    2,
                    32,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SarcoPlankAbility),
                    seImageNumber: 162
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SarcoPlankAbility : GenericAbility
    {
        private int OriginalAgility;

        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            if (HostShip.AssignedManeuver != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDefenseStart, AskToUseSarcoPlankAbility);
            }
        }

        private void AskToUseSarcoPlankAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                ShouldUseAbility,
                ChangeAgility,
                descriptionLong: "Do you want to treat your agility value as " + HostShip.AssignedManeuver.Speed + "?",
                imageHolder: HostShip
            );
        }

        private bool ShouldUseAbility()
        {
            return HostShip.AssignedManeuver.Speed > HostShip.State.Agility;
        }

        private void ChangeAgility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + "'s Agility is now " + HostShip.AssignedManeuver.Speed);

            OriginalAgility = HostShip.State.Agility;
            HostShip.ChangeAgilityBy(HostShip.AssignedManeuver.Speed - HostShip.State.Agility);

            HostShip.OnAttackFinishAsDefender += RestoreOriginalAgility;

            DecisionSubPhase.ConfirmDecision();
        }

        private void RestoreOriginalAgility(GenericShip ship)
        {
            HostShip.OnAttackFinishAsDefender -= RestoreOriginalAgility;

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + "'s Agility hass been restored to " + OriginalAgility);
            HostShip.ChangeAgilityBy(OriginalAgility - HostShip.AssignedManeuver.Speed);
        }
    }
}
