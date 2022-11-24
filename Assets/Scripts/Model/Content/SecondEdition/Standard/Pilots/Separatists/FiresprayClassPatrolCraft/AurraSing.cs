using BoardTools;
using Content;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class AurraSing : FiresprayClassPatrolCraft
        {
            public AurraSing() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Aurra Sing",
                    "Bane of the Jedi",
                    Faction.Separatists,
                    4,
                    8,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AurraSingAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    skinName: "Jango Fett"
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/da4433d1-3b24-4bcc-a335-d6d5810b596d/SWZ97_AurraSinglegal.png?format=1000w";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AurraSingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}