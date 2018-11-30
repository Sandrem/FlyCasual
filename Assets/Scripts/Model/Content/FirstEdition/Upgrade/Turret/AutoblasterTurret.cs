using Ship;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class AutoblasterTurret : GenericSpecialWeapon
    {
        public AutoblasterTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Autoblaster Turret",
                UpgradeType.Turret,
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 1,
                    canShootOutsideArc: true
                ),
                abilityType: typeof(Abilities.FirstEdition.AutoblasterAbility)
            );
        }        
    }
}