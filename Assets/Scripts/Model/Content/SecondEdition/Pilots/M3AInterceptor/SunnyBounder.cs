using Ship;
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
                    27,
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
        protected override void AddAbility(GenericShip ship)
        {
            if (Combat.CurrentDiceRoll.DiceList.All(die => die.Side == Combat.CurrentDiceRoll.DiceList.First().Side))
            {
                HostShip.AddAvailableDiceModificationOwn(
                    new ActionsList.FirstEdition.SunnyBounderAbilityAction(() => { IsAbilityUsed = true; HostShip = HostShip; })
                );
            }
        }
    }
}