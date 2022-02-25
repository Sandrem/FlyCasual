using System;
using System.Collections.Generic;
using System.Linq;
using BoardTools;
using Content;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class BombardmentDrone : HyenaClassDroidBomber
    {
        public BombardmentDrone()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Bombardment Drone",
                "Time on Target",
                Faction.Separatists,
                3,
                3,
                8,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.BombardmentDroneAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Device,
                    UpgradeType.Device
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );
            
            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/099422de35fb5ad2c2d238237e7dfe2c.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BombardmentDroneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombLaunchTemplates += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombLaunchTemplates -= CheckAbility;
        }

        private void CheckAbility(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> dropTemplates = HostShip.GetAvailableBombDropTemplates(upgrade);

            foreach (ManeuverTemplate template in dropTemplates)
            {
                if (!availableTemplates.Any(n => n.Name == template.Name))
                {
                    availableTemplates.Add(template);
                }
            }
        }
    }
}