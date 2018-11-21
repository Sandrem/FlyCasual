using Upgrade;

namespace Ship
{
    namespace FirstEdition.StarViper
    {
        public class BlackSunAssassin : StarViper
        {
            public BlackSunAssassin() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Sun Assassin",
                    5,
                    28
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Black Sun Assassin";
            }
        }
    }
}
