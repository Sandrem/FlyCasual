using Arcs;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YWing
    {
        public class Kavil : YWing
        {
            public Kavil() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kavil",
                    7,
                    24,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KavilAbility),
                    factionOverride: Faction.Scum
                );

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Astromech);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.SalvagedAstromech);

                ModelInfo.SkinName = "Kavil";
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
            if (!BoardTools.Board.IsShipInArcByType(HostShip, Combat.Defender, ArcType.Primary))
            {
                diceCount++;
            }
        }
    }
}

