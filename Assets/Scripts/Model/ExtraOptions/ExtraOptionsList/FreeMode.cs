namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class FreeModeExtraOption : ExtraOption
        {
            public FreeModeExtraOption()
            {
                Name = "Free Mode";
                Description = "You can install any upgrade on any ship";
            }

            protected override void Activate()
            {
                DebugManager.FreeMode = true;
            }

            protected override void Deactivate()
            {
                DebugManager.FreeMode = false;
            }
        }
    }
}
