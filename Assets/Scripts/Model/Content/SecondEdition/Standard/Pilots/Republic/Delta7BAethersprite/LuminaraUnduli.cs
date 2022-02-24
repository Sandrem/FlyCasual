using Content;
using System.Collections.Generic;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class LuminaraUnduli7B : Delta7BAethersprite
    {
        public LuminaraUnduli7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Luminara Unduli",
                "Wise Protector",
                Faction.Republic,
                4,
                6,
                7,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.LuminaraUnduliAbility),
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Green"
            );

            PilotNameCanonical = "luminaraunduli-delta7baethersprite";

            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/7f87b6c12631687bedf75a18582af0b0.png";
        }
    }
}