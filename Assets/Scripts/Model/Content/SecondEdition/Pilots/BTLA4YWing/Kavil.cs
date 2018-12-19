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
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KavilAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 165
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            if (Combat.ArcForShot.ArcType != Arcs.ArcType.Front)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": +1 attack die");
                diceCount++;
            }
        }
    }
}