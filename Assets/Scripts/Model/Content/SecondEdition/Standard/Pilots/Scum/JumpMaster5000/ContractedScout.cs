namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class ContractedScout : JumpMaster5000
        {
            public ContractedScout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Contracted Scout",
                    "",
                    Faction.Scum,
                    2,
                    5,
                    4,
                    seImageNumber: 217
                );
            }
        }
    }
}