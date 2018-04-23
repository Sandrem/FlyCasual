using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class WeaponsEngineer : GenericUpgrade
    {
        public WeaponsEngineer() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Weapons Engineer";
            Cost = 3;

            IsHidden = true;

            AvatarOffset = new Vector2(60, 1);
        }
    }
}
