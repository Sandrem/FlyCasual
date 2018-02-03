using Ship;
using Ship.Firespray31;
using Upgrade;
using Mods.ModsList;
using System.Collections.Generic;

namespace UpgradesList
{
    public class KSEPursuitSpecial : GenericUpgradeSlotUpgrade
    {
        public bool IsAlwaysUse;

        public KSEPursuitSpecial() : base()
        {
            Type = UpgradeType.Title;
            Name = "KSE Pursuit Special";
            Cost = -2;

            ImageUrl = "https://i.imgur.com/TmDkcUR.png";

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Title) { MustBeDifferent = true }
            };

            FromMod = typeof(FiresprayFix);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}