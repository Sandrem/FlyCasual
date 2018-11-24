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
                    44,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 209
                );

                ModelInfo.SkinName = "Cartel Executioner";
            }
        }
    }
}