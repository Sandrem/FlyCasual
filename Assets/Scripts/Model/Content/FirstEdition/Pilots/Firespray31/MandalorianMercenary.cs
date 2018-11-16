using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class MandalorianMercenary : Firespray31
        {
            public MandalorianMercenary() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Mandalorian Mercenary",
                    5,
                    35
                );

                ModelInfo.SkinName = "Mandalorian Mercenary";

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;
            }
        }
    }
}
