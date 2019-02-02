using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ClusterMissiles : GenericSpecialWeapon
    {
        public ClusterMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Assault Missiles",
                UpgradeType.Missile,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true,
                    twinAttack: true
                )
            );
        }        
    }
}