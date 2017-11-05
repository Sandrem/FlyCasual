using Ship;
using Tokens;
using System.Collections.Generic;
using System.Linq;
using ActionsList;

namespace RulesList
{
    public class BullseyeArcRule
    {
        private List<System.Type> DiceModificationsForbidden = new List<System.Type>()
        {
            typeof(FocusAction),
            typeof(EvadeAction)
        };

        public BullseyeArcRule()
        {
            GenericShip.OnTryAddAvailableActionEffectGlobal += CheckBullseyeArc;
        }

        private void CheckBullseyeArc(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (DiceModificationsForbidden.Contains(action.GetType()))
            {
                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.ShipId == ship.ShipId)
                {
                    if (Combat.Attacker.ShipBaseArcsType == Arcs.BaseArcsType.ArcBullseye)
                    {
                        if (Combat.ShotInfo.InBullseyeArc)
                        {
                            Messages.ShowError("Bullseye: " + action.EffectName + " cannot be used");
                            canBeUsed = false;
                        }
                    }
                }
            }
        }

    }
}


