using Ship;
using Upgrade;

namespace UpgradesList
{
    public class Tactician : GenericUpgrade
    {
        public Tactician() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Tactician";
            Cost = 2;
            isLimited = true;            
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttackFinish += RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {            
            if (Combat.Attacker == Host && Combat.ShotInfo.InArc && Combat.ShotInfo.Range == 2)
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Tactician's ability",
                    TriggerType = TriggerTypes.OnAttackFinish,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = TacticianAbility
                });
        }

        private void TacticianAbility(object sender, System.EventArgs e)
        {
            Combat.Defender.AssignToken(new Tokens.StressToken(Combat.Defender), delegate
            {
                Messages.ShowInfo("Defender gained stress from Tactician");
                Triggers.FinishTrigger();
            });
        }
    }
}
