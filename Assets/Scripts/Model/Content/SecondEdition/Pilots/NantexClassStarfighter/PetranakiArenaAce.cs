﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class PetranakiArenaAce : NantexClassStarfighter
        {
            public PetranakiArenaAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Petranaki Arena Ace",
                    4,
                    38,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/77/2c/772cb855-8706-443e-ae92-8da68b1fdb74/swz47_cards-arena-ace.png";
            }
        }
    }
}