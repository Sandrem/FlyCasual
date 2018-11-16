using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class EzraBridger : TIELnFighter
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    3,
                    32,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    force: 1
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Force);

                ShipInfo.Faction = Faction.Rebel;

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";

                SEImageNumber = 46;
            }
        }
    }
}
