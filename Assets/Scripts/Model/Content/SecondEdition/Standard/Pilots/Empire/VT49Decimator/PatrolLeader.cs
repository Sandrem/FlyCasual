namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class PatrolLeader : VT49Decimator
        {
            public PatrolLeader() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Patrol Leader",
                    "",
                    Faction.Imperial,
                    2,
                    8,
                    22,
                    seImageNumber: 148
                );
            }
        }
    }
}
