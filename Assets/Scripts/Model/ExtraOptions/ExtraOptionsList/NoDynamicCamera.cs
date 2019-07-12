namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class NoDynamicCameraExtraOption : ExtraOption
        {
            public NoDynamicCameraExtraOption()
            {
                Name = "No Dynamic Camera";
                Description = "Turns off camera movement that shows attacks of ships.";
            }

            protected override void Activate()
            {
                DebugManager.NoCinematicCamera = true;
            }

            protected override void Deactivate()
            {
                DebugManager.NoCinematicCamera = false;
            }
        }
    }
}
