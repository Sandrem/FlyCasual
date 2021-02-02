using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class GrandMoffTarkin : GenericUpgrade
    {
        public GrandMoffTarkin() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Grand Moff Tarkin",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true
            );
        }        
    }
}