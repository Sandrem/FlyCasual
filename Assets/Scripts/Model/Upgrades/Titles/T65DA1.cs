using Ship;
using Ship.XWing;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;

namespace UpgradesList
{
    public class T65DA1 : GenericUpgradeSlotUpgrade
    {
        public T65DA1() : base()
        {
            FromMod = typeof(TitlesForClassicShips);

            Type = UpgradeType.Title;
            Name = "T-65D-A1";
            Cost = -2;

            ImageUrl = "https://i.imgur.com/BsCL0uj.png";

            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.System)
            };
            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Astromech
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is XWing);
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGetManeuverColorDecreaseComplexity += T65DA1Ability;
        }

        private void T65DA1Ability(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.ManeuverColor.None)
            {
                if (movement.Speed == Movement.ManeuverSpeed.Speed2)
                {
                    movement.ColorComplexity = Movement.ManeuverColor.Green;
                }
            }
        }
    }
}