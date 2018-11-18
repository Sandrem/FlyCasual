using Upgrade;

namespace Ship
{
    namespace FirstEdition.M12LKimogilaFighter
    {
        public class CartelExecutioner : M12LKimogilaFighter
        {
            public CartelExecutioner() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cartel Executioner",
                    5,
                    24
                );

                ModelInfo.SkinName = "Cartel Executioner";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}
