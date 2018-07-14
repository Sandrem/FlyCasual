using Ship;
using Ship.M12LKimogila;
using UnityEngine;
using Upgrade;
using Abilities;
using BoardTools;
using Arcs;
using Tokens;

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

            UpgradeAbilities.Add(new EnforcerAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is M12LKimogila;
        }
    }
}

namespace Abilities
{
    public class EnforcerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += TryRegisterStressEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= TryRegisterStressEffect;
        }

        private void TryRegisterStressEffect(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            if (shotInfo.InArcByType(ArcTypes.Bullseye))
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
            Combat.Attacker.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger
            );
        }
    }
}
