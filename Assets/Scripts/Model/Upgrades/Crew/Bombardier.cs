using Upgrade;
using Bombs;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

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

            UpgradeAbilities.Add(new BombardierAbility());
        }
    }
}

namespace Abilities
{
    public class BombardierAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates += BombardierTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates -= BombardierTemplate;
        }

        private void BombardierTemplate(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Straight_2)) availableTemplates.Add(BombDropTemplates.Straight_2);
        }
    }
}
