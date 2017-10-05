using Upgrade;

namespace UpgradesList
{
    public class RebelCaptive : GenericUpgrade
    {
        private bool IsUsed = false;

        public RebelCaptive() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Rebel Captive";
            Cost = 3;
            isUnique = true;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Empire;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttack += RegisterTrigger;

            Phases.OnEndPhaseStart += Cleanup;
        }

        private void RegisterTrigger()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Rebel Captive's ability",
                TriggerType = TriggerTypes.OnAttackStart,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = RebelCaptiveAbility
            });
        }
        
        public void RebelCaptiveAbility(object sender, System.EventArgs e)
        {
            if (!IsUsed && Combat.AttackStep == CombatStep.Attack && Combat.Defender == Host)
            {
                Messages.ShowInfoToHuman("Attacker gained stress from Rebel Captive");
                IsUsed = true;
                Combat.Attacker.AssignToken(new Tokens.StressToken(), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void Cleanup()
        {
            IsUsed = false;
        }
    }
}
