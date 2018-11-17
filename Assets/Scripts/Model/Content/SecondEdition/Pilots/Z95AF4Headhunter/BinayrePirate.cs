using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class BinayrePirate : Z95AF4Headhunter
        {
            public BinayrePirate() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Binayre Pirate",
                    1,
                    24
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 173;
            }
        }
    }
}
