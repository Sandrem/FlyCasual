namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class WorldOfPainExtraOption : ExtraOption
        {
            public WorldOfPainExtraOption()
            {
                Name = "World of Pain";
                Description = "All damage cards are dealt faceup.";
            }

            protected override void Activate()
            {
                DebugManager.DebugAllDamageIsCrits = true;
            }

            protected override void Deactivate()
            {
                DebugManager.DebugAllDamageIsCrits = false;
            }
        }
    }
}
