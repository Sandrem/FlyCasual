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

                ImageUrl = "http://infinitearenas.com/xw2browse/images/resistance/temmin-wexley.jpg";
            }
        }
    }
}
