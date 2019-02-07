using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonMissiles : GenericSpecialWeapon
    {
        public IonMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Missiles",
                UpgradeType.Missile,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.IonMissilesAbility),
                seImageNumber: 40
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IonMissilesAbility : Abilities.FirstEdition.IonDamageAbility
    {

        protected override void IonWeaponEffect(object sender, System.EventArgs e)
        {
            var ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            if (ionTokens > 0)
            {
                Combat.Defender.Tokens.AssignTokens(
                    () => new IonToken(Combat.Defender),
                    ionTokens,
                    delegate {
                        GameManagerScript.Wait(2, DefenderSuffersDamage);
                    }
                );
            }
            else
            {
                DefenderSuffersDamage();
            }
        }
    }

}