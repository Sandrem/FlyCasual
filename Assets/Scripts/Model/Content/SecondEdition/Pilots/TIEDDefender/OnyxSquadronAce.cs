using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class OnyxSquadronAce : TIEDDefender
        {
            public OnyxSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Onyx Squadron Ace",
                    4,
                    78,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 125
                );
            }
        }
    }
}
