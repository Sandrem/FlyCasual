using Upgrade;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class RedSquadronVeteran : T70XWing
        {
            public RedSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Red Squadron Veteran",
                    4,
                    26
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Red";
            }
        }
    }
}
