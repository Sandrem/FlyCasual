using Ship;
using System;
using System.Linq;

namespace Ship
{
    namespace M3AScyk
    {
        public class SunnyBounder : M3AScyk
        {
            public SunnyBounder() : base()
            {
                PilotName = "Sunny Bounder";
                PilotSkill = 1;
                Cost = 14;

                PilotAbilities.Add(new Abilities.SunnyBounderAbility());
                
                SkinName = "Sunny Bounder";
            }
        }
    }
}

namespace Abilities
{
    //Once per round, after you roll or reroll dice, if you have the same result on each of your dice, you may add 1 matching result.
    public class SunnyBounderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += AddAbility;
            HostShip.OnImmediatelyAfterReRolling += AddAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= AddAbility;
            HostShip.OnImmediatelyAfterReRolling -= AddAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        public void AddAbility(DiceRoll diceroll)
        {
            if (!IsAbilityUsed && diceroll.DiceList.All(die => die.Side == diceroll.DiceList.First().Side))
            {
                HostShip.AfterGenerateAvailableActionEffectsList += AddAvailableActionEffect;
            }
        }

        private void AddAvailableActionEffect(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.SunnyBounderAbilityAction(() => { IsAbilityUsed = true; }));
            HostShip.AfterGenerateAvailableActionEffectsList -= AddAvailableActionEffect;
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
            Name = EffectName = "Sunny Bounder ability";
            abilityIsUsed = setAbilityIsUsed;
        }

        public override int GetActionEffectPriority()
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
