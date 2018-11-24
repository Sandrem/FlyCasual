namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class ScimitarSquadronPilot : TIESaBomber, TIE
        {
            public ScimitarSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Scimitar Squadron Pilot",
                    2,
                    28,
                    seImageNumber: 112
                );
            }
        }
    }
}
