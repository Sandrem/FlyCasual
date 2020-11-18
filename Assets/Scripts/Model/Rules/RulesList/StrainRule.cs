using UnityEngine;
using Ship;
using Tokens;
using ActionsList;
using Players;
using Movement;
using System;
using System.Linq;

namespace RulesList
{
    public class StrainRule
    {
        public void CheckForStrainedDebuff(ref int count)
        {
            if (Combat.Defender.IsStrained)
            {
                Messages.ShowInfo("Strained: Defender rolls -1 defense die");
                Combat.Defender.Tokens.GetToken<StrainToken>().WasApplied = true;
                count--;
            }
        }

        public void TryRemoveAppliedStrainTokenAfterAttack(GenericShip ship)
        {
            if (Combat.Defender.IsStrained)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Remove Strain token",
                        TriggerOwner = Combat.Defender.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnAttackFinish,
                        EventHandler = delegate { RemoveAppliedStrainToken(Combat.Defender); }
                    }
                );
            }
        }

        private void RemoveAppliedStrainToken(GenericShip ship)
        {
            StrainToken appliedToken = ship.Tokens.GetTokens<StrainToken>().FirstOrDefault(n => n.WasApplied);
            
            if (appliedToken != null)
            {
                ship.Tokens.RemoveToken(typeof(StrainToken), Triggers.FinishTrigger);
            }
            else
            {
                // Sometimes Strain Token is assigned after defense dice roll and before finish of attack (example: Finn)
                Triggers.FinishTrigger();
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

