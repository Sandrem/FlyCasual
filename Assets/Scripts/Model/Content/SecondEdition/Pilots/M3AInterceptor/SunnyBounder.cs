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
                    30,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SunnyBounderAbility),
                    seImageNumber: 188
                );
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