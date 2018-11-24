using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class SabineWren : TIEFighter
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    5,
                    15,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenPilotAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    factionOverride: Faction.Rebel
                );

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}