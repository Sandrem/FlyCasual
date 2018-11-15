using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class GrandInquisitor : TIEAdvancedV1
        {
            public GrandInquisitor() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Grand Inquisitor",
                    5,
                    58,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.GrandInquisitorAbility),
                    force: 2
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Force);

                SEImageNumber = 99;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // Temporary implementation. If in the future other abilities manipulate range bonus this will need to change.
    public class GrandInquisitorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterInquisitorAttackAbility;
            HostShip.OnAttackStartAsDefender += RegisterInquisitorDefenseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterInquisitorAttackAbility;
            HostShip.OnAttackStartAsDefender -= RegisterInquisitorDefenseAbility;
        }

        private void RegisterInquisitorAttackAbility()
        {
            if (HostShip.State.Force < 1)
                return;

            if (Combat.ShotInfo.Range == 1)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(AlwaysUseByDefault, UseInquisitorAttackAbility);
            });
        }

        private void RegisterInquisitorDefenseAbility()
        {
            if (HostShip.State.Force < 1)
                return;

            if (Combat.ShotInfo.Range > 1)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(AlwaysUseByDefault, UseInquisitorDefenseAbility);
            });
        }

        private void UseInquisitorAttackAbility(object sender, EventArgs e)
        {
            HostShip.State.Force--;
            HostShip.AfterGotNumberOfAttackDice += IncreaseAttackDice;
            DecisionSubPhase.ConfirmDecision();
        }

        private void UseInquisitorDefenseAbility(object sender, EventArgs e)
        {
            HostShip.State.Force--;
            Combat.Attacker.AfterGotNumberOfAttackDice += DecreaseAttackDice;
            DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseAttackDice(ref int result)
        {
            result++;
            HostShip.AfterGotNumberOfAttackDice -= IncreaseAttackDice;
        }

        private void DecreaseAttackDice(ref int result)
        {
            result--;
            Combat.Attacker.AfterGotNumberOfAttackDice -= DecreaseAttackDice;
        }
    }
}