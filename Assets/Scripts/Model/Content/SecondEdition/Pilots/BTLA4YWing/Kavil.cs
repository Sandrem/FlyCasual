using Arcs;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class Kavil : BTLA4YWing
        {
            public Kavil() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kavil",
                    5,
                    42,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KavilAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 165;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KavilAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += KavilPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= KavilPilotAbility;
        }

        private void KavilPilotAbility(ref int diceCount)
        {
            if (!BoardTools.Board.IsShipInArcByType(HostShip, Combat.Defender, ArcTypes.Primary))
            {
                diceCount++;
            }
        }
    }
}
