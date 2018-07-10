using Ship;
using System.Collections.Generic;
using ActionsList;
using Arcs;

namespace RulesList
{
    public class BullseyeArcRule
    {
        private List<System.Type> DiceModificationsForbidden = new List<System.Type>()
        {
            typeof(FocusAction),
            typeof(EvadeAction)
        };

        public void CheckBullseyeArc(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (DiceModificationsForbidden.Contains(action.GetType()))
            {
                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.ShipId == ship.ShipId)
                {
                    if (Combat.Attacker.ShipBaseArcsType == BaseArcsType.ArcBullseye)
                    {
                        if (Combat.ShotInfo.InArcByType(ArcTypes.Bullseye))
                        {
                            Messages.ShowError("Bullseye: " + action.DiceModificationName + " cannot be used");
                            canBeUsed = false;
                        }
                    }
                }
            }
        }

    }
}


