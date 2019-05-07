namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class NoCombatExtraOption : ExtraOption
        {
            public NoCombatExtraOption()
            {
                Name = "No Combat";
                Description = "Ships don't attack - only fly.";
            }

            protected override void Activate()
            {
                DebugManager.DebugNoCombat = true;
            }

            protected override void Deactivate()
            {
                DebugManager.DebugNoCombat = false;
            }
        }
    }
}
