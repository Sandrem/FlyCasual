using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class IonPulseMissiles : GenericSpecialWeapon
    {
        public IonPulseMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Pulse Missiles",
                UpgradeType.Missile,
                cost: 3,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.IonPulseMissilesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class IonPulseMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterIonPulseMissleHit;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnShotHitAsAttacker -= RegisterIonPulseMissleHit;
        }

        private void RegisterIonPulseMissleHit()
        {
            if (Combat.ChosenWeapon == this.HostUpgrade)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Ion Pulse Missile Hit",
                        TriggerType = TriggerTypes.OnShotHit,
                        TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                        EventHandler = delegate {
                            IonPulseMissilesHitEffect();
                        }
                    }
                );
            }
        }

        private void IonPulseMissilesHitEffect()
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Messages.ShowInfo("The defender receives 2 Ion tokens");

            Combat.Defender.Tokens.AssignToken(typeof(IonToken), AddSecondIonToken);
        }

        private void AddSecondIonToken()
        {
            Combat.Defender.Tokens.AssignToken(typeof(IonToken), DefenderSuffersDamage);
        }

        private void DefenderSuffersDamage()
        {
            Messages.ShowInfo("Defender suffers 1 damage");

            DamageSourceEventArgs ionpulseDamage = new DamageSourceEventArgs()
            {
                Source = "Ion Pulse Missiles",
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, ionpulseDamage, Triggers.FinishTrigger);

            HostShip.OnShotHitAsAttacker -= RegisterIonPulseMissleHit;
        }
    }
}