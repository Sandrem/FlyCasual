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
                    66
                );

                ModelInfo.SkinName = "Mandalorian Mercenary";
                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 154;
            }
        }
    }
}
