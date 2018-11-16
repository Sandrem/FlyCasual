namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class BountyHunter : Firespray31
        {
            public BountyHunter() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bounty Hunter",
                    3,
                    33
                );

                ModelInfo.SkinName = "Bounty Hunter";

                ShipInfo.Faction = Faction.Imperial;
            }
        }
    }
}
