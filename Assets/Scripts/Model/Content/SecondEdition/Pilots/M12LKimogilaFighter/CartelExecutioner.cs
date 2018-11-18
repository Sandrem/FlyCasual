using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class CartelExecutioner : M12LKimogilaFighter
        {
            public CartelExecutioner() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cartel Executioner",
                    3,
                    44
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Cartel Executioner";

                SEImageNumber = 209;
            }
        }
    }
}