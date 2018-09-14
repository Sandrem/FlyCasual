using UnityEngine;
using Upgrade;
using Ship;
using Movement;
using RuleSets;

namespace UpgradesList
{
    public class NienNunb : GenericUpgrade, ISecondEditionUpgrade
    {
        public NienNunb() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Nien Nunb";
            Cost = 1;

            isUnique = true;

            AvatarOffset = new Vector2(50, 1);

            UpgradeAbilities.Add(new Abilities.NienNunbCrewAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 5;

            UpgradeAbilities.RemoveAll(a => a is Abilities.NienNunbCrewAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.NienNunbCrewAbility());

            SEImageNumber = 90;
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

        private void NienNunbAbility(GenericShip ship, ref MovementStruct movement)
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
