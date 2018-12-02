using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEInterceptor
    {
        public class SaberSquadronPilot : TIEInterceptor
        {
            public SaberSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Saber Squadron Pilot",
                    4,
                    21,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Red Stripes";
            }
        }
    }
}