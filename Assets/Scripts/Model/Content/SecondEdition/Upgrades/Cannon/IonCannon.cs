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
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.FirstEdition.IonCannonAbility),
                seImageNumber: 28
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class IonCannonAbility : Abilities.FirstEdition.IonCannonAbility
    {

        protected override void IonCannonEffect(object sender, System.EventArgs e)
        {
            int ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(
                () => new IonToken(Combat.Defender),
                ionTokens,
                delegate
                {
                    GameManagerScript.Wait(2, DefenderSuffersDamage);
                }
            );
        }
    }
}