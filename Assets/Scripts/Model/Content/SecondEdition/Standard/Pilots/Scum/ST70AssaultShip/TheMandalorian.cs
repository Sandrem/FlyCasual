using Arcs;
using BoardTools;
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
                PilotInfo = new PilotCardInfo
                (
                    "The Mandalorian",
                    5,
                    55,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TheMandalorianAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://i.imgur.com/Ncx2wka.png";
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