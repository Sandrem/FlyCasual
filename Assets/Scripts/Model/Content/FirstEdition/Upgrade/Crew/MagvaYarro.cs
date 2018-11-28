using Ship;
using SubPhases;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class MagvaYarro : GenericUpgrade
    {
        public MagvaYarro() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Magva Yarro",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restrictionFaction: Faction.Rebel,
                abilityType: typeof(Abilities.FirstEdition.MagvaYarroCrewAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class MagvaYarroCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += AddMagvaYarroAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= AddMagvaYarroAbility;
        }

        private void AddMagvaYarroAbility(GenericShip ship)
        {
            if (Combat.Defender.ShipId == HostShip.ShipId)
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
            Messages.ShowInfo("Magva Yarro: Free Target Lock");
            ActionsHolder.AcquireTargetLock(Combat.Defender, Combat.Attacker, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}