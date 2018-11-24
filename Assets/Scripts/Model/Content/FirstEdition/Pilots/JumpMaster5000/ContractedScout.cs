using Upgrade;

namespace Ship
{
    namespace FirstEdition.JumpMaster5000
    {
        public class ContractedScout : JumpMaster5000
        {
            public ContractedScout() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Contracted Scout",
                    3,
                    25,
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}
