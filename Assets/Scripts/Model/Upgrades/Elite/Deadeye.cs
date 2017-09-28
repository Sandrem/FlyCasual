using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class Deadeye : GenericUpgrade
    {
        public Deadeye() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Deadeye";
            Cost = 1;
        }
    }
}
