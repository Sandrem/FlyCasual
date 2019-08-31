using UnityEngine;
using Ship;
using Tokens;
using ActionsList;
using Players;
using Movement;
using System;

namespace RulesList
{
    public class DepleteRule
    {
        public void CheckForDepletedDebuff(ref int count)
        {
            if (Combat.Attacker.IsDepleted)
            {
                Messages.ShowInfo("Depleted: Attacker rolls -1 attack die");
                count--;
            }
        }

        public void TryRemoveDepleteTokenAfterAttack(GenericShip ship)
        {
            if (Combat.Attacker.IsDepleted)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Remove Deplete token",
                        TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnAttackFinish,
                        EventHandler = delegate { RemoveDepleteToken(Combat.Attacker); }
                    }
                );
            }
        }

        public void TryRemoveDepleteTokenAfterManeuver(GenericShip ship)
        {
            if (Selection.ThisShip.IsDepleted && Selection.ThisShip.GetLastManeuverColor() == MovementComplexity.Easy)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Remove Deplete token",
                        TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnMovementExecuted,
                        EventHandler = delegate { RemoveDepleteToken(Selection.ThisShip); }
                    }
                );
            }
        }

        private void RemoveDepleteToken(GenericShip ship)
        {
            ship.Tokens.RemoveToken(typeof(DepleteToken), Triggers.FinishTrigger);
        }
    }
}

