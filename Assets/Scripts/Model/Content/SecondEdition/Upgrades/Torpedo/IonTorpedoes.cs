using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class IonTorpedoes : GenericSpecialWeapon
    {
        public IonTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ion Torpedoes",
                UpgradeType.Torpedo,
                cost: 6,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 2
                ),
                abilityType: typeof(Abilities.SecondEdition.IonTorpedoesAbility),
                seImageNumber: 34
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IonTorpedoesAbility : Abilities.FirstEdition.IonDamageAbility
    {

        protected override void IonTurretEffect(object sender, System.EventArgs e)
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