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
                    70,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TestPilotBlackoutAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "http://infinitearenas.com/xw2browse/images/first-order/blackout.jpg";
            }
        }
    }
}
