using Ship;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace FirstEdition.M3AInterceptor
    {
        public class SunnyBounder : M3AInterceptor
        {
            public SunnyBounder() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sunny Bounder",
                    1,
                    14,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SunnyBounderAbility)
                );

                ModelInfo.SkinName = "Sunny Bounder";
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
            ship.AddAvailableDiceModification(new ActionsList.FirstEdition.SunnyBounderAbilityAction(() => { IsAbilityUsed = true; }));
            HostShip.OnGenerateDiceModifications -= AddAvailableActionEffect;
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
