namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class ZebOrrelios : TIEFighter
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeb\" Orrelios",
                    3,
                    13,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ZebOrreliosPilotAbility),
                    factionOverride: Faction.Rebel
                );

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}