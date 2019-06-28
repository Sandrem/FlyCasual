using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAgAggressor
    {
        public class OnyxSquadronScout : TIEAgAggressor, TIE
        {
            public OnyxSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Onyx Squadron Scout",
                    3,
                    30,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 129
                );
            }
        }
    }
}
