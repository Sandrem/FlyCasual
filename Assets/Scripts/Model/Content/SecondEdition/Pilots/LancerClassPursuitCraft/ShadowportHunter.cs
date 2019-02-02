namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class ShadowportHunter : LancerClassPursuitCraft
        {
            public ShadowportHunter() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Shadowport Hunter",
                    2,
                    60,
                    seImageNumber: 221
                );
            }
        }
    }
}