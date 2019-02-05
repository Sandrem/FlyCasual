namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class DeltaSquadronPilot : TIEDDefender
        {
            public DeltaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Delta Squadron Pilot",
                    1,
                    70,
                    seImageNumber: 126
                );
            }
        }
    }
}
