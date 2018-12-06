using Ship;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;
using Arcs;

namespace UpgradesList.FirstEdition
{
    public class KSEPursuitSpecial : GenericUpgrade
    {
        public KSEPursuitSpecial() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "KSE Pursuit Special",
                UpgradeType.Title,
                cost: -2,          
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Firespray31.Firespray31)),
                addSlot: new UpgradeSlot(UpgradeType.Title) { MustBeDifferent = true },
                abilityType: typeof(Abilities.FirstEdition.KSEPursuitSpecialAbility)
            );

            FromMod = typeof(FiresprayFix);
            ImageUrl = "https://i.imgur.com/TmDkcUR.png";
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class KSEPursuitSpecialAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            ToggleAbility(true);
        }

        public override void DeactivateAbility()
        {
            ToggleAbility(false);
        }

        private void ToggleAbility(bool isActive)
        {
            GenericArc rearArc = HostShip.ArcsInfo.GetArc<ArcRear>();

            rearArc.ShotPermissions.CanShootCannon = isActive;
            rearArc.ShotPermissions.CanShootTorpedoes = isActive;
            rearArc.ShotPermissions.CanShootMissiles = isActive;
        }
    }
}