using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class GavinSykes : NabooRoyalN1Starfighter
        {
            public GavinSykes() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Gavin Sykes",
                    "Bravo Six",
                    Faction.Republic,
                    3,
                    4,
                    16,
                    isLimited: true,
                    abilityType: typeof(GavinSykesAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/d857e3ca-7688-4854-9787-8f051dec8144/SWZ97_GavynSykeslegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GavinSykesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
