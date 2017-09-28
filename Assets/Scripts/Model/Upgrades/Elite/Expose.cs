using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class Expose : GenericUpgrade
    {
        public Expose() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Expose";
            Cost = 4;
        }
    }
}
