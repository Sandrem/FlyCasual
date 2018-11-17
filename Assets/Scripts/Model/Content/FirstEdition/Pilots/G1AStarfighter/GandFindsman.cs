namespace Ship
{
    namespace FirstEdition.G1AStarfighter
    {
        public class GandFindsman : G1AStarfighter
        {
            public GandFindsman() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gand Findsman",
                    5,
                    25
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
