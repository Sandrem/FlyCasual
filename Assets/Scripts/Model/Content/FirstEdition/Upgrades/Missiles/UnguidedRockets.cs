using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class UnguidedRockets : GenericSpecialWeapon
    {
        public UnguidedRockets() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Unguided Rockets",
                types: new List<UpgradeType>(){
                    UpgradeType.Missile,
                    UpgradeType.Missile
                },
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3,
                    requiresToken: typeof(FocusToken)
                )
            );
        }        
    }
}

// TODO
// * Do not allow target lock to spend to reroll
// * Do not allow defender to modifiy attack dice
// * Only allow focus modification 