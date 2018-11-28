using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class RebelCaptive : GenericUpgrade
    {
        public RebelCaptive() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rebel Captive",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restrictionFaction: Faction.Imperial,
                abilityType: typeof(Abilities.FirstEdition.RebelCaptiveAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class RebelCaptiveAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += RegisterTrigger;
            Phases.Events.OnEndPhaseStart_NoTriggers += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= RegisterTrigger;
            Phases.Events.OnEndPhaseStart_NoTriggers -= ClearIsAbilityUsedFlag;
        }

        private void RegisterTrigger()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Rebel Captive's ability",
                TriggerType = TriggerTypes.OnAttackStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = DoRebelCaptiveAbility
            });
        }

        public void DoRebelCaptiveAbility(object sender, System.EventArgs e)
        {
            if (!IsAbilityUsed && Combat.AttackStep == CombatStep.Attack && Combat.Defender == HostShip)
            {
                Messages.ShowInfoToHuman("Attacker gained stress from Rebel Captive");
                IsAbilityUsed = true;
                Combat.Attacker.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
