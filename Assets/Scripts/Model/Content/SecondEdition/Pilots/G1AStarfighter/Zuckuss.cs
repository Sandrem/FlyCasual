using Upgrade;

namespace Ship
{
    namespace SecondEdition.G1AStarfighter
    {
        public class Zuckuss : G1AStarfighter
        {
            public Zuckuss() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zuckuss",
                    3,
                    47,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ZuckussAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 202
                );
            }
        }
    }
}