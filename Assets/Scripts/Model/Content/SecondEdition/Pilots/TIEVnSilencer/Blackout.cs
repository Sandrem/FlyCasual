using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class Blackout : TIEVnSilencer
        {
            public Blackout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Blackout\"",
                    5,
                    63,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TestPilotBlackoutAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/c5659b210e13b4e11fdd5f1396f2847c.png";
            }
        }
    }
}
