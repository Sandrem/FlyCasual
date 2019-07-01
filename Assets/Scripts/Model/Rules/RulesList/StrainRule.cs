using UnityEngine;
using Ship;
using Tokens;
using ActionsList;
using Players;
using Movement;
using System;

namespace RulesList
{
    public class StrainRule
    {
        public void CheckForStrainedDebuff(ref int count)
        {
            if (Combat.Defender.IsStrained)
            {
                Messages.ShowInfo("Strained: Defender rolls -1 defense die");
                count--;
            }
        }

        public void TryRemoveStrainTokenAfterAttack(GenericShip ship)
        {
            if (Combat.Defender.IsStrained)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Remove Strain token",
                        TriggerOwner = Combat.Defender.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnAttackFinish,
                        EventHandler = delegate { RemoveStrainToken(Combat.Defender); }
                    }
                );
            }
        }

        public void TryRemoveStrainTokenAfterManeuver(GenericShip ship)
        {
            if (Selection.ThisShip.IsStrained && Selection.ThisShip.GetLastManeuverColor() == MovementComplexity.Easy)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Remove Strain token",
                        TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnMovementExecuted,
                        EventHandler = delegate { RemoveStrainToken(Selection.ThisShip); }
                    }
                );
            }
        }

        private void RemoveStrainToken(GenericShip ship)
        {
            ship.Tokens.RemoveToken(typeof(StrainToken), Triggers.FinishTrigger);
        }
    }
}

