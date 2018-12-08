using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class NienNunb : T70XWing
        {
            public NienNunb() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Nien Nunb",
                    5,
                    55,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.NienNunbAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                //seImageNumber: 93
                );

                //ModelInfo.SkinName = "Black One";

                ImageUrl = "http://infinitearenas.com/xw2browse/images/resistance/nien-nunb.jpg";
            }
        }
    }
}
