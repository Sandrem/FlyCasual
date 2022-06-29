using Arcs;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ST70AssaultShip
    {
        public class TheMandalorian : ST70AssaultShip
        {
            public TheMandalorian() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "The Mandalorian",
                    "Din Djarin",
                    Faction.Scum,
                    5,
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TheMandalorianAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Cannon,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian,
                        Tags.BountyHunter
                    }
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/bb/Themandalorian.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TheMandalorianAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        private bool IsAvailable()
        {
            return IsInFrontSectorOf2Ships()
                && DiceRoll.CurrentDiceRoll.Blanks > 0;
        }

        private bool IsInFrontSectorOf2Ships()
        {
            int count = 0;

            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                int rangeInFrontSector = enemyShip.SectorsInfo.RangeToShipBySector(HostShip, ArcType.Front);
                if (rangeInFrontSector >= 1 && rangeInFrontSector <= 2)
                {
                    count++;
                    if (count == 2) return true;
                }
            }

            return false;
        }

        private int GetAiPriority()
        {
            return 100; // Free change limited by side if 1
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}