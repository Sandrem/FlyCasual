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

        private bool AbilityShouldTrigger()
        {
            if (Combat.Attacker == Host)
            {
                Board.ShipDistanceInformation shipDistance = new Board.ShipDistanceInformation(Host, Combat.Defender);
                if (shipDistance.Range <= 2)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, Combat.Defender, Combat.ChosenWeapon);
                    if (shotInfo.InArc)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void RegisterTrigger(GenericShip ship)
        {            
            if (AbilityShouldTrigger())
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
            Combat.Defender.AssignToken(new Tokens.StressToken(), delegate
            {
                Messages.ShowInfo("Defender gained stress from Tactician");
                Triggers.FinishTrigger();
            });
        }
    }
}
