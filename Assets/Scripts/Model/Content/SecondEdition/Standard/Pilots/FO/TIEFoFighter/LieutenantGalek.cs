using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class LieutenantGalek : TIEFoFighter
        {
            public LieutenantGalek() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Galek",
                    "Harsh Instructor",
                    Faction.FirstOrder,
                    5,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantGalekAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/fa44b5e8-b315-48cb-97cd-ad7250ae3ef2/SWZ97_LieutenantGaleklegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantGalekAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
