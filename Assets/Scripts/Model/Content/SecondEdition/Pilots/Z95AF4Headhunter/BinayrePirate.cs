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
                    24,
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum,
                    seImageNumber: 173
                );

                ModelInfo.SkinName = "Binayre Pirate";
            }
        }
    }
}
