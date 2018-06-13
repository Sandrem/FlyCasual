using BoardTools;
using Tokens;

namespace ActionsList
{

    public class ReinforceAftAction : GenericReinforceAction
    {

        public ReinforceAftAction()
        {
            Name = EffectName = "Reinforce (Aft)";
            Facing = Arcs.ArcFacing.Rear180;
        }

        public override void ActionTake()
        {
            base.ActionTake();
            Host.Tokens.AssignToken(typeof(ReinforceAftToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            result = 25 + 30*Actions.CountEnemiesTargeting(Host, -1);

            return result;
        }

    }

}
