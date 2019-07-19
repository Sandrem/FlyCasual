using Bombs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesList
{
    public class FuseRule
    {
        public void CheckForRemoveFuseInsteadOfDetonating(GenericDeviceGameObject deviceObject)
        {
            if (deviceObject.IsFused)
            {
                Messages.ShowInfoToHuman($"{BombsManager.CurrentDevice.UpgradeInfo.Name} removes one Fuse token instead of detonating.");
                deviceObject.Fuses--;
                BombsManager.DetonationIsAllowed = false;
            }
        }
    }
}
