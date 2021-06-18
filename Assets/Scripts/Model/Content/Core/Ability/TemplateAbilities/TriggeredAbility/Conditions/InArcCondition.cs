using Arcs;
using BoardTools;

namespace Abilities
{
    public class InArcCondition : Condition
    {
        public InArcCondition(ArcType arcType)
        {
            ArcType = arcType;
        }

        public ArcType ArcType { get; }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null || args.ShipAbilityHost == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            ShotInfo shotInfo = new ShotInfo(args.ShipAbilityHost, args.ShipToCheck, args.ShipAbilityHost.PrimaryWeapons);
            return shotInfo.InArcByType(ArcType);
        }
    }
}
