using Ship;
using Upgrade;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class FireControlSystem : GenericUpgrade
    {
        public FireControlSystem() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fire-Control System",
                UpgradeType.Sensor,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.FireControlSystemAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
                if (!Combat.Defender.IsDestroyed)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAcquireTargetLock);
                }
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                AcquireTargetLock,
                descriptionLong: "Do you want to acquire a Target Lock on the defender?",
                imageHolder: HostUpgrade,
                showAlwaysUseOption: true
            );
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Fire-Control System grants " + Combat.Attacker.PilotInfo.PilotName + " a free Target Lock on " + Combat.Defender.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(Combat.Attacker, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}