namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class NoObstaclesExtraOption : ExtraOption
        {
            public NoObstaclesExtraOption()
            {
                Name = "No Obstacles";
                Description = "Skip obstacles placement phase";
            }

            protected override void Activate()
            {
                DebugManager.NoObstaclesSetup= true;
            }

            protected override void Deactivate()
            {
                DebugManager.NoObstaclesSetup = false;
            }
        }
    }
}
