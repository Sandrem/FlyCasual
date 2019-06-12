using Arcs;
using Movement;
using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CombatConfiguration : GenericUpgrade
    {
        public CombatConfiguration() : base()
        {
            FromMod = typeof(Mods.ModsList.JumpMasterConfigurationsMod);

            UpgradeInfo = new UpgradeCardInfo(
                "Combat Configuration",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.JumpMaster5000.JumpMaster5000)),
                abilityType: typeof(Abilities.SecondEdition.CombatConfigurationAbility),
                addArc: new ShipArcInfo(ArcType.Front, 2)
            );

            ImageUrl = "https://i.imgur.com/BLDJH5K.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CombatConfigurationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += ApplyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= ApplyAbility;
        }

        private void ApplyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Direction == ManeuverDirection.Right)
            {
                if (movement.Bearing == ManeuverBearing.Bank || movement.Bearing == ManeuverBearing.Turn)
                {
                    movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
                }
            }
        }
    }
}