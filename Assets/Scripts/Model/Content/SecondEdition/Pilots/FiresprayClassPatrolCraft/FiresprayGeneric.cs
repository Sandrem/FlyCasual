namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class FiresprayGeneric : FiresprayClassPatrolCraft
        {
            public FiresprayGeneric() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Firespray Generic",
                    2,
                    62,
                    factionOverride: Faction.Separatists
                );
            }
        }
    }
}
