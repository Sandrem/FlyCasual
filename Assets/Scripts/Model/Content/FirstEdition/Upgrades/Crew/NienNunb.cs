using Ship;
using Upgrade;
using UnityEngine;
using Movement;

namespace UpgradesList.FirstEdition
{
    public class NienNunb : GenericUpgrade
    {
        public NienNunb() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Nien Nunb",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.NienNunbCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(50, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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

        private void NienNunbAbility(GenericShip ship, ref ManeuverHolder movement)
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