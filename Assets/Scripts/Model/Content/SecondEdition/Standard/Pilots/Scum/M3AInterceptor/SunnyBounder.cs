using Ship;
using System;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class SunnyBounder : M3AInterceptor
        {
            public SunnyBounder() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sunny Bounder",
                    "Incurable Optimist",
                    Faction.Scum,
                    1,
                    3,
                    5,
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

namespace Abilities.FirstEdition
{
    //Once per round, after you roll or reroll dice, if you have the same result on each of your dice, you may add 1 matching result.
    public class SunnyBounderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        protected virtual void AddAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && Combat.CurrentDiceRoll.DiceList.All(die => die.Side == Combat.CurrentDiceRoll.DiceList.First().Side))
            {
                HostShip.AddAvailableDiceModificationOwn
                (
                    new ActionsList.FirstEdition.SunnyBounderAbilityAction(() => { IsAbilityUsed = true; }) { ImageUrl = HostShip.ImageUrl }
                );
            }
        }
    }
}


namespace ActionsList.FirstEdition
{
    public class SunnyBounderAbilityAction : GenericAction
    {
        private Action abilityIsUsed;

        public SunnyBounderAbilityAction(Action setAbilityIsUsed)
        {
            Name = DiceModificationName = "Sunny Bounder";
            abilityIsUsed = setAbilityIsUsed;
        }

        public override int GetDiceModificationPriority()
        {
            return 110;
        }

        public override void ActionEffect(System.Action callBack)
        {
            abilityIsUsed();
            Combat.CurrentDiceRoll.AddDiceAndShow(Combat.CurrentDiceRoll.DiceList.First().Side);
            callBack();
        }
    }
}