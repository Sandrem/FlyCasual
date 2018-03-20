using Ship;
using Ship.StarViper;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;

namespace UpgradesList
{
    public class Virago : GenericUpgradeSlotUpgrade
    {
        public Virago() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Virago";
            Cost = 1;
            isUnique = true;

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.System),
                new UpgradeSlot(UpgradeType.Illicit)
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ((ship is StarViper) && (ship.PilotSkill > 3));
        }
    }
}
