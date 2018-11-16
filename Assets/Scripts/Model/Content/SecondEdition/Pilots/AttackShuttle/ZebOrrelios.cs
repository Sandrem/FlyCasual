namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class ZebOrrelios : AttackShuttle
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeb\" Orrelios",
                    2,
                    34,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ZebOrreliosPilotAbility)
                );

                SEImageNumber = 37;
            }
        }
    }
}