using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class IonCannon : GenericSpecialWeapon
    {
        public IonCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Cannon",
                UpgradeType.Cannon,
                cost: 3,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.FirstEdition.IonCannonAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class IonCannonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterIonCannonEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterIonCannonEffect;
        }

        private void RegisterIonCannonEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, IonCannonEffect);
            }
        }

        protected virtual void IonCannonEffect(object sender, System.EventArgs e)
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
            DamageSourceEventArgs ionCannonDamage = new DamageSourceEventArgs()
            {
                Source = Combat.Attacker,
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, ionCannonDamage, Triggers.FinishTrigger);
        }
    }

}