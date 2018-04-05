using Upgrade;
using Ship;

namespace UpgradesList
{
    public class RebelCaptive : GenericUpgrade
    {
        private bool IsUsed = false;

        public RebelCaptive() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Rebel Captive";
            Cost = 3;
            isUnique = true;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttackStartAsDefender += RegisterTrigger;

            Phases.OnEndPhaseStart_NoTriggers += Cleanup;
            Host.OnShipIsDestroyed += StopAbility;
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
                Combat.Attacker.Tokens.AssignToken(new Tokens.StressToken(Combat.Attacker), Triggers.FinishTrigger);
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

        private void StopAbility(GenericShip host, bool isFled)
        {
            Phases.OnEndPhaseStart_NoTriggers -= Cleanup;
        }
    }
}
