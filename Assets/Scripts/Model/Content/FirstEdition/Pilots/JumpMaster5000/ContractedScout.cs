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
                    25
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
