using Content;
using System.Collections.Generic;

namespace Ship.SecondEdition.Delta7BAethersprite
{
    public class SaeseeTiin7B : Delta7BAethersprite
    {
        public SaeseeTiin7B()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Saesee Tiin",
                "Prophetic Pilot",
                Faction.Republic,
                4,
                6,
                9,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.SaeseeTiinAbility),
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                legality: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                },
                skinName: "Saesee Tiin"
            );

            PilotNameCanonical = "saeseetiin-delta7baethersprite";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/77/73/777350cb-614b-48fd-ad8d-d9c867053c6b/swz32_saesee-tiin.png";
        }
    }
}
