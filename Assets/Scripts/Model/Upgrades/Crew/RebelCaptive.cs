using Upgrade;
using Ship;
using Abilities;

namespace UpgradesList
{
    public class RebelCaptive : GenericUpgrade
    {
        public RebelCaptive() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Rebel Captive";
            Cost = 3;
            isUnique = true;

            UpgradeAbilities.Add(new RebelCaptiveAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
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
                Combat.Attacker.Tokens.AssignToken(new Tokens.StressToken(Combat.Attacker), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
