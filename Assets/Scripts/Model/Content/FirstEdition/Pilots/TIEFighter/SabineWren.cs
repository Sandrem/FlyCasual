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
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.Faction = Faction.Rebel;
                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}