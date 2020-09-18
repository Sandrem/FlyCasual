using Ship;
using System.Collections.Generic;

namespace Abilities.Parameters
{
    public enum AiSelectShipTeamPriority
    {
        Enemy,
        Friendly
    }

    public enum AiSelectShipSpecial
    {
        None,
        Agile,
        Worst
    }

    public class AiSelectShipPlan
    {
        public AiSelectShipTeamPriority AiSelectShipTeamPriority { get; }
        public AiSelectShipSpecial AiSelectShipSpecial { get; }

        public AiSelectShipPlan(AiSelectShipTeamPriority aiSelectShipTeamPriority, AiSelectShipSpecial aiSelectShipSpecial = AiSelectShipSpecial.None)
        {
            AiSelectShipTeamPriority = aiSelectShipTeamPriority;
            AiSelectShipSpecial = aiSelectShipSpecial;
        }

        public int GetAiSelectShipPriority(GenericShip possibleTagetShip, GenericShip sourceShip)
        {
            int priority = GetInitialPriorityByCost(possibleTagetShip);

            switch (AiSelectShipTeamPriority)
            {
                case AiSelectShipTeamPriority.Enemy:
                    if (Tools.IsSameTeam(possibleTagetShip, sourceShip)) return -1;
                    break;
                case AiSelectShipTeamPriority.Friendly:
                    if (Tools.IsAnotherTeam(possibleTagetShip, sourceShip)) return -1;
                    break;
                default:
                    break;
            }

            switch (AiSelectShipSpecial)
            {
                case AiSelectShipSpecial.Agile:
                    if (possibleTagetShip.State.Agility == 0) priority -= 10;
                    break;
                default:
                    break;
            }

            return priority;
        }

        private int GetInitialPriorityByCost(GenericShip possibleTagetShip)
        {
            return (AiSelectShipSpecial == AiSelectShipSpecial.Worst) ? 100 - possibleTagetShip.PilotInfo.Cost : possibleTagetShip.PilotInfo.Cost;
        }
    }
}
