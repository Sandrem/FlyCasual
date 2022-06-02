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
                abilityType: typeof(Abilities.SecondEdition.IonDamageAbility),
                seImageNumber: 40
            );
        }
    }
}