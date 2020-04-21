namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class NoLoadingScreenTipsExtraOption : ExtraOption
        {
            public NoLoadingScreenTipsExtraOption()
            {
                Name = "No Loading Screen Tips";
                Description = "Don't show tips on loading screen - load next scene without few seconds of delay";
            }

            protected override void Activate()
            {
                LoadingScreen.DontWaitFewSeconds = true;
            }

            protected override void Deactivate()
            {
                LoadingScreen.DontWaitFewSeconds = false;
            }
        }
    }
}
