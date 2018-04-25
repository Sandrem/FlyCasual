using Abilities;
using Ship;
using SubPhases;
using Upgrade;

namespace UpgradesList
{
    public class MagvaYarro : GenericUpgrade
    {
        public MagvaYarro() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Magva Yarro";
            Cost = 2;

            isUnique = true;

            UpgradeAbilities.Add(new MagvaYarroAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class MagvaYarroAbility : GenericAbility
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
            Actions.AcquireTargetLock(Combat.Defender, Combat.Attacker, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}

