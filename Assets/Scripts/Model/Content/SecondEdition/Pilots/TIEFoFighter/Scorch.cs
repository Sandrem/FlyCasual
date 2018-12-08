using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Scorch : TIEFoFighter
        {
            public Scorch() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Scorch\"",
                    4,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ZetaLeaderAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/80/bb/80bbabbb-3b30-448e-a896-ddcfc05082bd/swz26_a1_scorch.png";
            }
        }
    }
}
