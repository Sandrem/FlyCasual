using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;
using BoardTools;
using Arcs;

namespace UpgradesList.FirstEdition
{
    public class Enforcer : GenericUpgrade
    {
        public Enforcer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Enforcer",
                UpgradeType.Title,
                cost: 1,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.M12LKimogilaFighter.M12LKimogilaFighter)),
                abilityType: typeof(Abilities.FirstEdition.EnforcerAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            if (Combat.Defender.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.Bullseye))
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
            Combat.Attacker.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}