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
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    force: 1,
                    extraUpgradeIcon: UpgradeType.Force,
                    factionOverride: Faction.Rebel,
                    seImageNumber: 46
                );

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}
