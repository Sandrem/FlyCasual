using BoardTools;
using System;
using System.Collections.Generic;

namespace Abilities
{
    public class TeamCondition : Condition
    {
        private Team.Type TeamType { get; }

        public TeamCondition(Team.Type teamType)
        {
            TeamType = teamType;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null || args.ShipAbilityHost == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            switch (TeamType)
            {
                case Team.Type.Friendly:
                    return args.ShipToCheck.Owner.PlayerNo == args.ShipAbilityHost.Owner.PlayerNo;
                case Team.Type.Enemy:
                    return args.ShipToCheck.Owner.PlayerNo != args.ShipAbilityHost.Owner.PlayerNo;
                default:
                    return false;
            }
        }
    }
}
