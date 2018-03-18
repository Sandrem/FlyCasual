using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class Daredevil : GenericUpgrade
    {
        public Daredevil() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Daredevil";
            Cost = 3;

            IsHidden = true;
        }
    }
}
