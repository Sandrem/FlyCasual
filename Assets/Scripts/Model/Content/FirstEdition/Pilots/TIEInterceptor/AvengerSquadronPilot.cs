namespace Ship
{
    namespace FirstEdition.TIEInterceptor
    {
        public class AvengerSquadronPilot : TIEInterceptor
        {
            public AvengerSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Avenger Squadron Pilot",
                    3,
                    20
                );
            }
        }
    }
}