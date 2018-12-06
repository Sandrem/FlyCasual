namespace Ship
{
    namespace FirstEdition.SheathipedeClassShuttle
    {
        public class ZebOrrelios : SheathipedeClassShuttle
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeb\" Orrelios",
                    3,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ZebOrreliosPilotAbility)
                );
            }
        }
    }
}