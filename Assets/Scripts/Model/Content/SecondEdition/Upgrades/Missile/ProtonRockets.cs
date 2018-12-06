using Arcs;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ProtonRockets : GenericSpecialWeapon
    {
        public ProtonRockets() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Rockets",
                UpgradeType.Missile,
                cost: 7,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 5,
                    minRange: 1,
                    maxRange: 2,
                    requiresToken: typeof(FocusToken),
                    charges: 1,
                    arc: ArcType.Bullseye
                ),
                seImageNumber: 41
            );
        }
    }
}