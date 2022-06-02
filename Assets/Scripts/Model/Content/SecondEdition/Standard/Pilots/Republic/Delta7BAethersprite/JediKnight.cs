using Content;
using System.Collections.Generic;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class JediKnight7B : Delta7BAethersprite
    {
        public JediKnight7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Jedi Knight",
                "",
                Faction.Republic,
                3,
                6,
                3,
                force: 1,
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                }
            );

            PilotNameCanonical = "jediknight-delta7baethersprite";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b0/b3/b0b3f463-a3ea-4fe6-be69-41afed1b4110/swz32_jedi-knight.png";
        }
    }
}