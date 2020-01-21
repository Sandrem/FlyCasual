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
                    31,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    force: 1,
                    extraUpgradeIcon: UpgradeType.ForcePower,
                    factionOverride: Faction.Rebel,
                    seImageNumber: 46
                );

                PilotNameCanonical = "ezrabridger-tielnfighter";

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}
