﻿using BoardTools;

namespace ActionsList
{

    public class ReinforceAftAction : GenericReinforceAction
    {

        public ReinforceAftAction()
        {
            Name = EffectName = "Reinforce (Aft)";
        }

        public override void ActionTake()
        {
            base.ActionTake();
            Selection.ThisShip.Tokens.AssignToken(new Tokens.ReinforceAftToken(Host), Phases.CurrentSubPhase.CallBack);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Defence)
            {
                ShotInfo reverseShotInfo = new ShotInfo(Host, Combat.Attacker, Host.PrimaryWeapon);
                result = !reverseShotInfo.InArc;
            }
            return result;
        }

        public override int GetActionPriority()
        {
            int result = 0;

            result = 25 + 30*Actions.CountEnemiesTargeting(Host, -1);

            return result;
        }

    }

}
