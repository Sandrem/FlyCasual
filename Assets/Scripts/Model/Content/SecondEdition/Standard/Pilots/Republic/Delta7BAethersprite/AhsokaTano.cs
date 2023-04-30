using Content;
using System.Collections.Generic;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class AhsokaTano7B : Delta7BAethersprite
    {
        public AhsokaTano7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Ahsoka Tano",
                "\"Snips\"",
                Faction.Republic,
                3,
                6,
                10,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AhsokaTanoAbility),
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Ahsoka Tano"
            );

            PilotNameCanonical = "ahsokatano-delta7baethersprite";
        }
    }
}