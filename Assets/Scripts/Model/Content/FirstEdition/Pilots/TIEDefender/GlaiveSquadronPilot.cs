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
                    34
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}
