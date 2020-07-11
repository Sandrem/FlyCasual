namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class AlternativeControlsExtraOption : ExtraOption
        {
            public AlternativeControlsExtraOption()
            {
                Name = "Alternative Camera Controls";
                Description = "Use X/Y axis contols instead of WASD. Can be useful in you don't use QWERTY keyboard.";
            }

            protected override void Activate()
            {
                DebugManager.AlternativeCameraControls = true;
            }

            protected override void Deactivate()
            {
                DebugManager.AlternativeCameraControls = false;
            }
        }
    }
}
