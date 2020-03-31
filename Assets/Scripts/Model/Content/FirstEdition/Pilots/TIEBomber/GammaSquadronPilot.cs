namespace Ship
{
    namespace FirstEdition.TIEBomber
    {
        public class GammaSquadronPilot : TIEBomber
        {
            public GammaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gamma Squadron Pilot",
                    4,
                    18
                );

                ModelInfo.SkinName = "Gamma Squadron";
            }
        }
    }
}
