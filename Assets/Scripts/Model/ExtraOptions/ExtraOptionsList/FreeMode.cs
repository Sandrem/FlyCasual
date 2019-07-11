namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class FreeModeExtraOption : ExtraOption
        {
            public FreeModeExtraOption()
            {
                Name = "Free Mode";
                Description = "You can install any upgrade on any ship.\n" +
                    "Adds \"Improved Initiative\" upgrade that increases Initiative of pilot by 1.";
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
