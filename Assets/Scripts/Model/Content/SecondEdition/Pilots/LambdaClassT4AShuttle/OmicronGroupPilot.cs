namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class OmicronGroupPilot : LambdaClassT4AShuttle
        {
            public OmicronGroupPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Omicron Group Pilot",
                    1,
                    43,
                    seImageNumber: 145
                );
            }
        }
    }
}
