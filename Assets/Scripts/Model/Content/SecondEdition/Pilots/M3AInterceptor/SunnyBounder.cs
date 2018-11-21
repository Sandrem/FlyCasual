using System.Linq;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class SunnyBounder : M3AInterceptor
        {
            public SunnyBounder() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sunny Bounder",
                    1,
                    31,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.SunnyBounderAbility)
                );

                ModelInfo.SkinName = "Sunny Bounder";

                SEImageNumber = 188;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SunnyBounderAbility : Abilities.FirstEdition.SunnyBounderAbility
    {
        // No more "once per round".
        protected override void AddAbility(DiceRoll diceroll)
        {
            if (diceroll.DiceList.All(die => die.Side == diceroll.DiceList.First().Side))
            {
                HostShip.OnGenerateDiceModifications += AddAvailableActionEffect;
            }
        }
    }
}