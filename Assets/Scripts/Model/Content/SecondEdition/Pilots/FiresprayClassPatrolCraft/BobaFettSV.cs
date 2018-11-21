using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BobaFettSV : FiresprayClassPatrolCraft
        {
            public BobaFettSV() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    5,
                    80,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettScumAbility)
                );

                ModelInfo.SkinName = "Boba Fett";
                ShipInfo.Faction = Faction.Scum;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 149;
            }
        }
    }
}