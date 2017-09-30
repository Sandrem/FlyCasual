using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class Daredevil : GenericUpgrade
    {
        public Daredevil() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Daredevil";
            Cost = 3;
        }
    }
}
