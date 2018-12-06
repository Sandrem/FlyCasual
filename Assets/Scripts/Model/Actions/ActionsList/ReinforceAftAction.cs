using BoardTools;
using Tokens;

namespace ActionsList
{

    public class ReinforceAftAction : GenericReinforceAction
    {

        public ReinforceAftAction()
        {
            Name = DiceModificationName = "Reinforce (Aft)";
            Facing = Arcs.ArcFacing.FullRear;
        }

        public override void ActionTake()
        {
            base.ActionTake();
            HostShip.Tokens.AssignToken(typeof(ReinforceAftToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            result = 25 + 30*ActionsHolder.CountEnemiesTargeting(HostShip, -1);

            return result;
        }

    }

}
