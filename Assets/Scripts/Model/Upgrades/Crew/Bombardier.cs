using Upgrade;
using Bombs;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList
{
    public class Bombardier : GenericUpgrade
    {
        public Bombardier() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Bombardier";
            Cost = 1;

            AvatarOffset = new Vector2(23, 1);
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnGetAvailableBombDropTemplates += BombardierTemplate;
        }

        private void BombardierTemplate(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Straight_2)) availableTemplates.Add(BombDropTemplates.Straight_2);
        }
    }
}
