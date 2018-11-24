using Upgrade;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class PoeDameronHotr : T70XWing
        {
            public PoeDameronHotr() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Poe Dameron (HotR)",
                    9,
                    33,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.PoeDameronAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Black One";
            }
        }
    }
}