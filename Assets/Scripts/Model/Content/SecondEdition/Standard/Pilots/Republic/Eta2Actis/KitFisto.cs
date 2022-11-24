using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class KitFisto : Eta2Actis
    {
        public KitFisto()
        {
            IsWIP = true;

            PilotInfo = new PilotCardInfo25
            (
                "Kit Fisto",
                "Enthusiastic Exemplar",
                Faction.Republic,
                4,
                4,
                8,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.KitFistoPilotAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                }
            );

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/53a01dab-f036-4231-92b6-2f5a2cccd184/SWZ97_KitFistolegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KitFistoPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
