namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class DynamicCameraExtraOption : ExtraOption
        {
            public DynamicCameraExtraOption()
            {
                Name = "Dynamic Camera";
                Description = "Camera that moves to show attacks of ships.";
            }

            protected override void Activate()
            {
                DebugManager.CinematicCamera = true;
            }

            protected override void Deactivate()
            {
                DebugManager.CinematicCamera = false;
            }
        }
    }
}
