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
                    33,
                    factionOverride: Faction.Imperial
                );

                ModelInfo.SkinName = "Bounty Hunter";
            }
        }
    }
}
