using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using Movement;

namespace UpgradesList
{
    public class NienNunb : GenericUpgrade
    {
        public NienNunb() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Nien Nunb";
            Cost = 1;

            isUnique = true;

            AvatarOffset = new Vector2(50, 1);

            UpgradeAbilities.Add(new NienNunbCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class NienNunbCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += NienNunbAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= NienNunbAbility;
        }

        private void NienNunbAbility(GenericShip ship, ref MovementStruct movement)
        {
            if (movement.ColorComplexity != MovementComplexity.None)
            {
                if (movement.Bearing == ManeuverBearing.Straight)
                {
                    movement.ColorComplexity = MovementComplexity.Easy;
                }
            }
        }
    }
}
