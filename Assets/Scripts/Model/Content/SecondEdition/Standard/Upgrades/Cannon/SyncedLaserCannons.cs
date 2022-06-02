using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SyncedLaserCannons : GenericSpecialWeapon
    {
        public SyncedLaserCannons() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Synced Laser Cannons",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Cannon,
                    UpgradeType.Cannon
                },
                cost: 8,
                weaponInfo: new SyncedLaserCannonsWeaponInfo(this)                
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/9b/11/9b115a04-a49d-40f0-ac06-5accb903aa5c/swz71_upgrade_synced-cannons.png";
        }

        private class SyncedLaserCannonsWeaponInfo : SpecialWeaponInfo
        {
            private GenericUpgrade HostUpgrade;
            public SyncedLaserCannonsWeaponInfo(GenericUpgrade hostUpgrade) : base(3, 2, 3)
            {
                HostUpgrade = hostUpgrade;
            }
                
            public override bool NoRangeBonus 
            { 
                get 
                {
                    if (Combat.AttackStep == CombatStep.Defence
                        && Combat.Attacker == HostUpgrade.HostShip
                        && HostUpgrade.HostShip.Tokens.HasToken<Tokens.CalculateToken>())
                        return true;
                    else 
                        return false;
                } 
            }
        
        }
    }
}