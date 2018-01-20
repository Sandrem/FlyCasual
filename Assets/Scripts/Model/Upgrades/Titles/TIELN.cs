using Ship;
using Ship.TIEFighter;
using Upgrade;
using Mods.ModsList;

namespace UpgradesList
{
    public class TIELN : GenericUpgradeSlotUpgrade
    {
        public bool IsAlwaysUse;

        public TIELN() : base()
        {
            FromMod = typeof(TitlesForClassicShips);

            Type = UpgradeType.Title;
            Name = "TIE/LN";
            Cost = -3;

            ImageUrl = "https://i.imgur.com/NuF578k.png";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEFighter;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.ChangeHullBy(-1);
        }
    }
}