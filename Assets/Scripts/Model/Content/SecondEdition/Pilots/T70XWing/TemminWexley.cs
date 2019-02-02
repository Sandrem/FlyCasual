using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class TemminWexley : T70XWing
        {
            public TemminWexley() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Temmin Wexley",
                    4,
                    54,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SnapWexleyAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                //seImageNumber: 93
                );

                //ModelInfo.SkinName = "Black One";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/bb98b7ea3a580542b586a9999fd352c9.png";
            }
        }
    }
}
