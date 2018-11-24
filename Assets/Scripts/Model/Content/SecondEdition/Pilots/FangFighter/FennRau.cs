using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class FennRau : FangFighter
        {
            public FennRau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fenn Rau",
                    6,
                    68,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.FennRauScumAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 155
                );
            }
        }
    }
}