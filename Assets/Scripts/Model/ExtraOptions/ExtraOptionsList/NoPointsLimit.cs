namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class NoPointsLimitExtraOption : ExtraOption
        {
            public NoPointsLimitExtraOption()
            {
                Name = "No points limit";
                Description = "You can use squads with total points cost more than " + Editions.Edition.Current.MaxPoints + ".\n"
                    + "Max 10 ships per player.";
            }

            protected override void Activate()
            {
                DebugManager.DebugNoSquadPointsLimit = true;
            }

            protected override void Deactivate()
            {
                DebugManager.DebugNoSquadPointsLimit = false;
            }
        }
    }
}
