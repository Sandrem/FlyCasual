using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class SkullSquadronPilot : FangFighter
        {
            public SkullSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Skull Squadron Pilot",
                    4,
                    47,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 159
                );

                ModelInfo.SkinName = "Skull Squadron";
            }
        }
    }
}