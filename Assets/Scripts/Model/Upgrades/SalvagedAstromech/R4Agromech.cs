using Abilities;
using System;
using Upgrade;
using Ship;
using Tokens;
using SubPhases;

namespace UpgradesList
{
    public class R4Agromech : GenericUpgrade
    {
        public R4Agromech() : base()
        {
            Types.Add(UpgradeType.SalvagedAstromech);
            Name = "R4 Agromech";
            Cost = 2;

            UpgradeAbilities.Add(new R4AgromechAbility());
        }
    }
}

namespace Abilities
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
            Actions.AcquireTargetLock(Combat.Attacker, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);            
        }
    }
}