using BoardTools;
using Content;
using Movement;
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
                PilotInfo = new PilotCardInfo25
                (
                    "\"Echo\"",
                    "Slippery Trickster",
                    Faction.Imperial,
                    4,
                    6,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EchoAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    seImageNumber: 132,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Echo";
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
