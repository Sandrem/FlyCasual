using Ship;
using Ship.XWing;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;
using Abilities;

namespace UpgradesList
{
    public class T65DA1 : GenericUpgradeSlotUpgrade
    {
        public T65DA1() : base()
        {
            FromMod = typeof(TitlesForClassicShips);

            Types.Add(UpgradeType.Title);
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

            UpgradeAbilities.Add(new T65DA1Ability());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is XWing);
        }
    }
}

namespace Abilities
{
    public class T65DA1Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckT65DA1Ability;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckT65DA1Ability;
        }

        private void CheckT65DA1Ability(GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if (movement.Speed == Movement.ManeuverSpeed.Speed2)
                {
                    movement.ColorComplexity = Movement.MovementComplexity.Easy;
                }
            }
        }
    }
}