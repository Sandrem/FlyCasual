namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class BlueSquadronRecruit : RZ2AWing
        {
            public BlueSquadronRecruit() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Blue Squadron Recruit",
                    "",
                    Faction.Resistance,
                    1,
                    4,
                    4,
                    skinName: "Blue"
                );
            }
        }
    }
}