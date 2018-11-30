using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class BlasterTurret : GenericSpecialWeapon
    {
        public BlasterTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Blaster Turret",
                UpgradeType.Turret,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2,
                    requiresToken: typeof(FocusToken),
                    spendsToken: typeof(FocusToken),
                    canShootOutsideArc: true
                )
            );
        }        
    }
}