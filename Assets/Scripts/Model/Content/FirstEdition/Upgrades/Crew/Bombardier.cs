using Ship;
using Upgrade;
using UnityEngine;
using Bombs;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class Bombardier : GenericUpgrade
    {
        public Bombardier() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Bombardier",
                UpgradeType.Crew,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.BombardierAbility)
            );

            Avatar = new AvatarInfo(Faction.None, new Vector2(23, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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