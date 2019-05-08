using Ship;
using SubPhases;
using UnityEngine;
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
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.MagvaYarroCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(87, 3));
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
                if (!Combat.Defender.IsDestroyed)
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
            Messages.ShowInfo("Magva Yarro allows " + Combat.Defender.PilotInfo.PilotName + " to acquire a Target Lock on " + Combat.Attacker.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(Combat.Defender, Combat.Attacker, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}