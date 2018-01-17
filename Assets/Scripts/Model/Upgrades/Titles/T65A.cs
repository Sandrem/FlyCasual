using Ship;
using Ship.XWing;
using Upgrade;
using Mods.ModsList;
using System.Collections.Generic;

namespace UpgradesList
{
    public class T65A : GenericUpgradeSlotUpgrade
    {
        public T65A() : base()
        {
            FromMod = typeof(TitlesForClassicShips);

            Types.Add(UpgradeType.Title);
            Name = "T-65A";
            Cost = 0;

            ImageUrl = "https://i.imgur.com/dWGkMD6.png";

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Modification) { MustBeDifferent = true }
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is XWing);
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);
        }
    }
}