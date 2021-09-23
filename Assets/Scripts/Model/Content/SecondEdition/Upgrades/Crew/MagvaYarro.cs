using Ship;
using SubPhases;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class MagvaYarro : GenericUpgrade
    {
        public MagvaYarro() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Magva Yarro",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.MagvaYarroCrewAbility),
                seImageNumber: 89
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(431, 1),
                new Vector2(125, 125)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class MagvaYarroCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsDefender += PlanTargetLockDecision;
            
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsDefender -= PlanTargetLockDecision;
            HostShip.OnAttackFinishAsDefender -= AddMagvaYarroAbility;
        }

        private void PlanTargetLockDecision()
        {
            HostShip.OnAttackFinishAsDefender += AddMagvaYarroAbility;
        }

        private void AddMagvaYarroAbility(GenericShip ship)
        {
            HostShip.OnAttackFinishAsDefender -= AddMagvaYarroAbility;
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAcquireTargetLock);
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                AcquireTargetLock,
                descriptionLong: "Do you want to acquire a lock on the attacker?",
                imageHolder: HostUpgrade,
                showAlwaysUseOption: true
            );
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Magva Yarro allows " + Combat.Defender.PilotInfo.PilotName + " to acquire a Target Lock on " + Combat.Attacker.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(Combat.Defender, Combat.Attacker, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}