namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class NoSquadBuilderLimitsExtraOption : ExtraOption
        {
            public NoSquadBuilderLimitsExtraOption()
            {
                Name = "No squad builder limits";
                Description = "You can use squads with total points cost more than " + Editions.Edition.Current.MaxPoints + " (Max 10 ships per player).\n"
                    + "You can use less than 3 ships in squad.";
            }

            protected override void Activate()
            {
                DebugManager.DebugNoSquadBuilderLimits = true;
            }

            protected override void Deactivate()
            {
                DebugManager.DebugNoSquadBuilderLimits = false;
            }
        }
    }
}
