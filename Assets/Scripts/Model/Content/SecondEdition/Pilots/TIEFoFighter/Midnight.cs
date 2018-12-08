using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Midnight : TIEFoFighter
        {
            public Midnight() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Midnight\"",
                    6,
                    44,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.OmegaLeaderAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/56/94/56940164-d919-4b04-8303-f39357555fad/swz18_a1_midnight.png";
            }
        }
    }
}
