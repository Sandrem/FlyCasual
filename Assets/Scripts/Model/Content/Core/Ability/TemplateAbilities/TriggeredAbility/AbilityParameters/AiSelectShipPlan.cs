using Ship;
using System.Collections.Generic;

namespace Abilities.Parameters
{
    public enum AiSelectShipTeamPriority
    {
        Enemy
    }

    public enum AiSelectShipSpecial
    {
        Agile
    }

    public class AiSelectShipPlan
    {
        public AiSelectShipTeamPriority AiSelectShipTeamPriority { get; }
        public AiSelectShipSpecial AiSelectShipSpecial { get; }

        public AiSelectShipPlan(AiSelectShipTeamPriority aiSelectShipTeamPriority, AiSelectShipSpecial aiSelectShipSpecial)
        {
            AiSelectShipTeamPriority = aiSelectShipTeamPriority;
            AiSelectShipSpecial = aiSelectShipSpecial;
        }

        public int GetAiSelectShipPriority(GenericShip possibleTagetShip, GenericShip sourceShip)
        {
            int priority = possibleTagetShip.PilotInfo.Cost;

            switch (AiSelectShipTeamPriority)
            {
                case AiSelectShipTeamPriority.Enemy:
                    if (Tools.IsSameTeam(possibleTagetShip, sourceShip)) return -1;
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
    }
}
