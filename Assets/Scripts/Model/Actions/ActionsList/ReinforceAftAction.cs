using BoardTools;

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
            Selection.ThisShip.Tokens.AssignToken(new Tokens.ReinforceAftToken(Host), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            result = 25 + 30*Actions.CountEnemiesTargeting(Host, -1);

            return result;
        }

    }

}
