using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class CardiaAcademyPilot : TIERbHeavy
        {
            public CardiaAcademyPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Cardia Academy Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    5,
                    8,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/43/2d/432db246-0a8d-42de-9711-a893c825b9b3/swz67_carida-cadet.png";
            }
        }
    }
}