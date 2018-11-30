using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class FlechetteCannon : GenericSpecialWeapon
    {
        public FlechetteCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Flechette Cannon",
                UpgradeType.Cannon,
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.FirstEdition.FlechetteCannonAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class FlechetteCannonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterFlechetteCannonEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterFlechetteCannonEffect;
        }

        private void RegisterFlechetteCannonEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, FlechetteCannonEffect);
            }
        }

        private void FlechetteCannonEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            DefenderSuffersDamage();
        }

        private void DefenderSuffersDamage()
        {
            Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            var trigger = RegisterAbilityTrigger(TriggerTypes.OnDamageIsDealt, Combat.Defender.SufferDamage, new DamageSourceEventArgs()
            {
                Source = Combat.Attacker,
                DamageType = DamageTypes.ShipAttack
            });
            trigger.Skippable = true;

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, CheckStress);
        }

        private void CheckStress()
        {
            if (!Combat.Defender.Tokens.HasToken(typeof(StressToken)))
            {
                Combat.Defender.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }
}