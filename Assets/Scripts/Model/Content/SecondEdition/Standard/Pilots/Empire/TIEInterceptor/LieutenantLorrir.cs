using BoardTools;
using Content;
using Movement;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class LieutenantLorrir : TIEInterceptor
        {
            public LieutenantLorrir() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Lorrir",
                    "Requiem for Brentaal",
                    Faction.Imperial,
                    3,
                    4,
                    6,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantLorrirAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    abilityText: "While you barrel roll, you may use bank templates, instead of straight template",
                    skinName: "Skystrike Academy"
                );

                ImageUrl = "https://i.imgur.com/5bOPsfP.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantLorrirAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates += ChangeBarrelRollTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates -= ChangeBarrelRollTemplates;
        }

        private void ChangeBarrelRollTemplates(List<ManeuverTemplate> availableTemplates)
        {
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1));
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1));
            availableTemplates.RemoveAll(n => n.Name == "Straight 1");
        }
    }
}