using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonCannon : GenericSpecialWeapon
    {
        public IonCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Cannon",
                UpgradeType.Cannon,
                cost: 6,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.IonDamageAbility),
                seImageNumber: 28
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IonDamageAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterIonWeaponEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterIonWeaponEffect;
        }

        protected void RegisterIonWeaponEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ion weapon effect",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = IonWeaponEffect
                });
            }
        }

        protected virtual void IonWeaponEffect(object sender, System.EventArgs e)
        {
            int ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            if (ionTokens > 0)
            {
                DefenderSuffersDamage(delegate {
                    Combat.Defender.Tokens.AssignTokens(
                        () => new IonToken(Combat.Defender),
                        ionTokens,
                        delegate {
                            GameManagerScript.Wait(2, Triggers.FinishTrigger);
                        }
                    );
                });
            }
            else
            {
                DefenderSuffersDamage(Triggers.FinishTrigger);
            }
        }

        protected void DefenderSuffersDamage(System.Action callback)
        {
            DamageSourceEventArgs ionWeaponDamage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, ionWeaponDamage, callback);
        }
    }

}