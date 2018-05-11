using Ship;
using UnityEngine;

namespace RulesList
{
    public class ForceRule
    {

        public void RegenerateForce(GenericShip ship)
        {
            if (ship.Force < ship.MaxForce) ship.Force++;
        }

        public void AddForceAction(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.ForceAction() { Host = ship });
        }
    }
}

namespace ActionsList
{

    public class ForceAction : GenericAction
    {

        public ForceAction()
        {
            Name = EffectName = "Force";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
            Host.Force--;
            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            return Host.Force > 0;
        }

        public override int GetActionEffectPriority()
        {
            // TODO: Add AI

            return 0;
        }

    }

}
