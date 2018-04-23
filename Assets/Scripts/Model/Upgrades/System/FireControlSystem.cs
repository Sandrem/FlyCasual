using Abilities;
using Ship;
using SubPhases;
using Upgrade;

namespace UpgradesList
{
    public class FireControlSystem : GenericUpgrade
    {
        public FireControlSystem() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Fire-Control System";
            Cost = 2;
            UpgradeAbilities.Add(new FireControlSystemAbility());
        }                
    }
}

namespace Abilities
{
    public class FireControlSystemAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += AddFireControlSystemAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= AddFireControlSystemAbility;
        }

        private void AddFireControlSystemAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId)
            {
                if (!(Combat.Defender.IsDestroyed || Combat.Defender.IsReadyToBeDestroyed))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAcquireTargetLock);
                }
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, AcquireTargetLock, null, null, true);
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Fire-Control System: Free Target Lock");
            Actions.AcquireTargetLock(Combat.Attacker, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}

