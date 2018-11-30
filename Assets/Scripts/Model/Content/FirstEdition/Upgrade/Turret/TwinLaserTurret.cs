using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class TwinLaserTurret : GenericSpecialWeapon
    {
        public TwinLaserTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Twin Laser Turret",
                UpgradeType.Turret,
                cost: 6,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    canShootOutsideArc: true,
                    twinAttack: true
                ),
                abilityType: typeof(Abilities.FirstEdition.TwinLaserTurretAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class TwinLaserTurretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterTwinLaserTurretEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterTwinLaserTurretEffect;
        }

        private void RegisterTwinLaserTurretEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        TriggerType = TriggerTypes.OnShotHit,
                        TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                        EventHandler = TwinLaserTurretEffect
                    }
                );
            }
        }
        private void TwinLaserTurretEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();
            DefenderSuffersDamage();
        }
        private void DefenderSuffersDamage()
        {
            DamageSourceEventArgs tltDamage = new DamageSourceEventArgs()
            {
                Source = "Twin Laser Turret",
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, tltDamage, Triggers.FinishTrigger);
        }
    }
}