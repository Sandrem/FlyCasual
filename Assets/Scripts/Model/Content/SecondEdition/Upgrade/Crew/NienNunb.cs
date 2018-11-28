using Ship;
using Upgrade;
using UnityEngine;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class NienNunb : GenericUpgrade
    {
        public NienNunb() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Nien Nunb",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restrictionFaction: Faction.Rebel,
                abilityType: typeof(Abilities.SecondEdition.NienNunbCrewAbility),
                seImageNumber: 90
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(50, 1));
        }        
    }
}

namespace Abilities.SecondEdition
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
                if (movement.Bearing == ManeuverBearing.Bank)
                {
                    movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
                }
            }
        }
    }
}