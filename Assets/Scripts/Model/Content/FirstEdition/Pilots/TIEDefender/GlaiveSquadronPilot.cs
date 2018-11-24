using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEDefender
    {
        public class GlaiveSquadronPilot : TIEDefender
        {
            public GlaiveSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Glaive Squadron Pilot",
                    6,
                    34,
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}
