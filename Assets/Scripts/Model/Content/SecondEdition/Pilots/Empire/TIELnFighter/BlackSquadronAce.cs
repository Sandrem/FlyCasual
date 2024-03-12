﻿using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class BlackSquadronAce : TIELnFighter
        {
            public BlackSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Squadron Ace",
                    "",
                    Faction.Imperial,
                    3,
                    2,
                    0,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 90
                );
            }
        }
    }
}
