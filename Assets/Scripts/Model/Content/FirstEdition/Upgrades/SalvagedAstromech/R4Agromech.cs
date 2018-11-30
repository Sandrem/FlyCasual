using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Tokens;
using System;
using Ship;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class R4Agromech : GenericUpgrade
    {
        public R4Agromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R4 Agromech",
                UpgradeType.SalvagedAstromech,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.R4AgromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class R4AgromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, Type type)
        {
            if (Combat.AttackStep == CombatStep.Attack && type == typeof(FocusToken) && Combat.Attacker == HostShip && Combat.Defender != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, AskAcquireTargetLock);
            }
        }

        private void AskAcquireTargetLock(object sender, EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, AcquireTargetLock, null, null, true);
        }

        private void AcquireTargetLock(object sender, EventArgs e)
        {
            ActionsHolder.AcquireTargetLock(Combat.Attacker, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}