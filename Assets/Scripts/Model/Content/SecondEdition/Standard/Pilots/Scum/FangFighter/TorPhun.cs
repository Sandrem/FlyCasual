using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class TorPhun : FangFighter
        {
            public TorPhun() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tor Phun",
                    "Direct Pressure",
                    Faction.Scum,
                    3,
                    5,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TorPhunAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    skinName: "Skull Squadron"
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/bbede7a9-748c-4269-8d6c-cdab20cc7029/SWZ97_TorPhunlegal.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TorPhunAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}