using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BountyHunter : FiresprayClassPatrolCraft
        {
            public BountyHunter() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bounty Hunter",
                    2,
                    62,
                    extraUpgradeIcon: UpgradeType.Crew,
                    seImageNumber: 154
                );

                ModelInfo.SkinName = "Mandalorian Mercenary";
            }
        }
    }
}
