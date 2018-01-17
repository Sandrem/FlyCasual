using Ship;
using Ship.M12LKimogila;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Enforcer : GenericUpgrade
    {
        public Enforcer() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Enforcer";
            Cost = 1;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is M12LKimogila;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnAttackFinish += TryRegisterStressEffect;
        }

        private void TryRegisterStressEffect(GenericShip ship)
        {
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            if (shotInfo.InBullseyeArc)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Endorcer's ability",
                    TriggerType = TriggerTypes.OnAttackFinish,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = StressEffect
                });
            }
        }

        private void StressEffect(object sender, System.EventArgs e)
        {
            Messages.ShowError("Enforcer: stress is assigned to the attacker");
            Combat.Attacker.AssignToken(new Tokens.StressToken(), Triggers.FinishTrigger);
        }
    }
}
