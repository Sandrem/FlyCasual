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

namespace Abilities.FirstEdition
{
    //Once per round, after you roll or reroll dice, if you have the same result on each of your dice, you may add 1 matching result.
    public class SunnyBounderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += AddAbility;
            HostShip.OnImmediatelyAfterReRolling += AddAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= AddAbility;
            HostShip.OnImmediatelyAfterReRolling -= AddAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        protected virtual void AddAbility(DiceRoll diceroll)
        {
            if (!IsAbilityUsed && diceroll.DiceList.All(die => die.Side == diceroll.DiceList.First().Side))
            {
                HostShip.OnGenerateDiceModifications += AddAvailableActionEffect;
            }
        }

        protected void AddAvailableActionEffect(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.SunnyBounderAbilityAction(() => { IsAbilityUsed = true; }));
            HostShip.OnGenerateDiceModifications -= AddAvailableActionEffect;
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

namespace ActionsList
{
    public class SunnyBounderAbilityAction : GenericAction
    {
        private Action abilityIsUsed;

        public SunnyBounderAbilityAction(Action setAbilityIsUsed)
        {
            Name = DiceModificationName = "Sunny Bounder ability";
            abilityIsUsed = setAbilityIsUsed;
        }

        public override int GetDiceModificationPriority()
        {
            return 110;
        }

        public override void ActionEffect(System.Action callBack)
        {
            abilityIsUsed();
            Combat.CurrentDiceRoll.AddDice(Combat.CurrentDiceRoll.DiceList.First().Side).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            callBack();
        }
    }
}