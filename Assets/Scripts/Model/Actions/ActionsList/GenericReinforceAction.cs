using Ship;
using System;
using System.Linq;
using UnityEngine;

namespace ActionsList
{

    public class GenericReinforceAction : GenericAction
    {
        public Arcs.ArcFacing Facing;

        public GenericReinforceAction()
        {
            Name = EffectName = "Reinforce (Generic)";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReinforceAction.png";
        }

        public override void ActionTake()
        {
            Host.OnImmediatelyAfterRolling += ApplyReinforceEffect;
            Host.OnRoundEnd += ClearReinforceEffect;
        }

        private void ClearReinforceEffect(GenericShip ship)
        {
            Host.OnImmediatelyAfterRolling -= ApplyReinforceEffect;
            ship.OnRoundEnd -= ClearReinforceEffect;
        }

        protected virtual void ApplyReinforceEffect(DiceRoll diceroll)
        {
            if (diceroll.Type == DiceKind.Defence && diceroll.CheckType == DiceRollCheckType.Combat)
            {
                if (RuleSets.RuleSet.Instance.ReinforceEffectCanBeUsed(Facing))
                {
                    Messages.ShowInfo("Reinforce: Evade result is added");
                    diceroll.AddDice(DieSide.Success).ShowWithoutRoll();
                    diceroll.OrganizeDicePositions();
                    diceroll.UpdateDiceCompareHelperPrediction();
                }
            }
        }

        public override bool IsActionAvailable()
        {
            bool result = true;
            if (Host.IsAlreadyExecutedAction<ReinforceForeAction>() || Host.IsAlreadyExecutedAction<ReinforceAftAction>())
            {
                result = false;
            }
            return result;
        }

    }

}
