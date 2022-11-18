using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class HondoOhnaka : FiresprayClassPatrolCraft
        {
            public HondoOhnaka() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Hondo Ohnaka",
                    "I Smell Profit!",
                    Faction.Scum,
                    1,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HondoOhnakaPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    skinName: "Jango Fett"
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/f0da444b-7695-43ad-b637-e7918f33a83c/SWZ97_HondoOhnakalegal.png";

                RequiredMods = new List<System.Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HondoOhnakaPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}