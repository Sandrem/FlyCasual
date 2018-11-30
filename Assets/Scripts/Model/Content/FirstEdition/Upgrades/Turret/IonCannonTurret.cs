using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class IonCannonTurret : GenericSpecialWeapon
    {
        public IonCannonTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Cannon Turret",
                UpgradeType.Turret,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2,
                    canShootOutsideArc: true
                ),
                abilityType: typeof(Abilities.FirstEdition.IonDamageAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class IonDamageAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterIonTurretEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterIonTurretEffect;
        }

        protected void RegisterIonTurretEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ion Cannon Turret effect",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = IonTurretEffect
                });
            }
        }

        protected virtual void IonTurretEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignToken(
                typeof(IonToken),
                delegate {
                    GameManagerScript.Wait(2, DefenderSuffersDamage);
                }
            );
        }

        protected void DefenderSuffersDamage()
        {
            DamageSourceEventArgs ionturretDamage = new DamageSourceEventArgs()
            {
                Source = "Ion Cannon Turret",
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, ionturretDamage, Triggers.FinishTrigger);
        }
    }

}