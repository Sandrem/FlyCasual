using ActionsList;
using Bombs;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class Deathfire : TIESaBomber, TIE
        {
            public Deathfire() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Deathfire\"",
                    2,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DeathfireAbility),
                    seImageNumber: 110
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeathfireAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsDestroyed -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, bool isFled)
        {
            if (!isFled)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, AskDeathfireAbility);
            }
        }

        private void AskDeathfireAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            AskToUseAbility(NeverUseByDefault, DropBomb, infoText: "Do you want to drop a bomb?");
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            BombsManager.CheckBombDropAvailability(HostShip, TriggerTypes.OnAbilityDirect);
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }
    }
}