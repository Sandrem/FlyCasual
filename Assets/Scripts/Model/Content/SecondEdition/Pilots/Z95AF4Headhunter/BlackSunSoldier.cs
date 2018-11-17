using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class BlackSunSoldier : Z95AF4Headhunter
        {
            public BlackSunSoldier() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Black Sun Soldier",
                    3,
                    27
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 172;
            }
        }
    }
}
