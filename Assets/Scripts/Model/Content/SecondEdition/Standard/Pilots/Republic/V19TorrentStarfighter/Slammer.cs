using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Slammer : V19TorrentStarfighter
    {
        public Slammer()
        {
            IsWIP = true;

            PilotInfo = new PilotCardInfo25
            (
                "\"Slammer\"",
                "Blue Three",
                Faction.Republic,
                1,
                3,
                7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.SlammerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                }
            );

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/d8a4e985-c13e-440c-bcdf-2d0aab767c6e/SWZ97_Slammerlegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SlammerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}
