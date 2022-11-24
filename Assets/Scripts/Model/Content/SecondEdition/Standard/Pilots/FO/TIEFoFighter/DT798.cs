using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class DT798 : TIEFoFighter
        {
            public DT798() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "DT-798",
                    "Jace Rucklin",
                    Faction.FirstOrder,
                    4,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DT798PilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/ad1d0d9c-9706-4e50-8d3b-8cd40877ea34/SWZ97_DT798legal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DT798PilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
