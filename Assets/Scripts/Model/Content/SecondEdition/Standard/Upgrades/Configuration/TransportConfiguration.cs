using Actions;
using ActionsList;
using Movement;
using Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TransportConfiguration : GenericUpgrade
    {
        public TransportConfiguration() : base()
        {
            FromMod = typeof(Mods.ModsList.JumpMasterConfigurationsMod);

            UpgradeInfo = new UpgradeCardInfo(
                "Transport Configuration",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.JumpMaster5000.JumpMaster5000)),
                abilityType: typeof(Abilities.SecondEdition.TransportConfigurationAbility),
                addAction: new ActionInfo(typeof(BarrelRollAction), source: this)
            );

            ImageUrl = "https://i.imgur.com/Ls4K4g3.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class TransportConfigurationAbility : GenericAbility
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
            if (movement.Direction == ManeuverDirection.Left)
            {
                if (movement.Bearing == ManeuverBearing.Bank || movement.Bearing == ManeuverBearing.Turn || movement.Bearing == ManeuverBearing.SegnorsLoop)
                {
                    movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
                }
            }
        }
    }
}