using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Static : TIEFoFighter
        {
            public Static() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Static\"",
                    4,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.OmegaAceAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/75/01/7501b0b3-6350-4f5a-af84-0c988a5493ba/swz26_a1_static.png";
            }
        }
    }
}
