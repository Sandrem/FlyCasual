namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class ZebOrrelios : TIELnFighter
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeb\" Orrelios",
                    2,
                    26,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ZebOrreliosPilotAbility)
                );

                ShipInfo.Faction = Faction.Rebel;

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";

                SEImageNumber = 49;
            }
        }
    }
}