namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class StraightToCombatExtraOption : ExtraOption
        {
            public StraightToCombatExtraOption()
            {
                Name = "Straight To Combat";
                Description = "Ships start in attack range and don't move.\nUse with \"No Obstacles\".";

                IsAvailable = !DebugManager.ReleaseVersion;
            }

            protected override void Activate()
            {
                DebugManager.DebugStraightToCombat = true;
            }

            protected override void Deactivate()
            {
                DebugManager.DebugStraightToCombat = false;
            }
        }
    }
}
