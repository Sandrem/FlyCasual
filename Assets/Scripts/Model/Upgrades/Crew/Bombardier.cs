using Upgrade;
using Bombs;
using System.Collections.Generic;

namespace UpgradesList
{
    public class Bombardier : GenericUpgrade
    {
        public Bombardier() : base()
        {
            Type = UpgradeType.Crew;
            Name = ShortName = "Bombardier";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Crew/bombardier.png";
            Cost = 1;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnGetAvailableBombDropTemplates += BombardierTemplate;
        }

        private void BombardierTemplate(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Straight2)) availableTemplates.Add(BombDropTemplates.Straight2);
        }
    }
}
