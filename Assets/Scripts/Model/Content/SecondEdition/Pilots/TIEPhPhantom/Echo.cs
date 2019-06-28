using BoardTools;
using Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class Echo : TIEPhPhantom
        {
            public Echo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Echo\"",
                    4,
                    51,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EchoAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 132
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EchoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableDecloakTemplates += ChangeDecloakTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableDecloakTemplates -= ChangeDecloakTemplates;
        }

        private void ChangeDecloakTemplates(List<ManeuverTemplate> availableTemplates)
        {
            if (availableTemplates.Any(n => n.Name == "Straight 2"))
            {
                availableTemplates.RemoveAll(n => n.Name == "Straight 2");
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed2));
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed2));
            }
        }
    }
}
