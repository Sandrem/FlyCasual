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
                abilityType: typeof(Abilities.FirstEdition.IonDamageAbility)
            );
        }        
    }
}