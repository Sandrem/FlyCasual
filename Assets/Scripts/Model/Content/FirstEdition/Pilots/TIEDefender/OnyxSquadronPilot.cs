using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEDefender
    {
        public class OnyxSquadronPilot : TIEDefender
        {
            public OnyxSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Onyx Squadron Pilot",
                    3,
                    32
                );
            }
        }
    }
}
