using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class DrawTheirFire : GenericUpgrade
    {
        public DrawTheirFire() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Draw Their Fire";
            Cost = 1;
        }
    }
}
