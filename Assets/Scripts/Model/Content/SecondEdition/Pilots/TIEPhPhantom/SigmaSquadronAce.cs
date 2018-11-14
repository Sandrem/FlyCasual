namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class SigmaSquadronAce : TIEPhPhantom
        {
            public SigmaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sigma Squadron Ace",
                    4,
                    46
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 133;
            }
        }
    }
}
