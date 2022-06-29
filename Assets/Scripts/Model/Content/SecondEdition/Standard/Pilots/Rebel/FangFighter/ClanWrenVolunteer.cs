using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class ClanWrenVolunteer : FangFighter
        {
            public ClanWrenVolunteer() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Clan Wren Volunteer",
                    "Unlikely Ally",
                    Faction.Rebel,
                    3,
                    5,
                    10,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.ClanWrenVolunteerAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Clan Wren Volunteers"
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/clanwrenvolunteer.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ClanWrenVolunteerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}