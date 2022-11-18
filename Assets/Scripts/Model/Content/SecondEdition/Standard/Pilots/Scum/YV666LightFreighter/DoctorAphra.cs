using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class DoctorAphra : YV666LightFreighter
        {
            public DoctorAphra() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Doctor Aphra",
                    "Professional Disaster Zone",
                    Faction.Scum,
                    3,
                    6,
                    16,
                    charges: 3,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DoctorAphraPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.BountyHunter
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/cab18bcf-9835-49d1-840b-82cc8f5ac976/SWZ97_DoctorAphralegal.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DoctorAphraPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
