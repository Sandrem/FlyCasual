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
                    66,
                    seImageNumber: 154
                );

                ModelInfo.SkinName = "Mandalorian Mercenary";
            }
        }
    }
}
