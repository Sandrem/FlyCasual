using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ResistanceTransport
    {
        public class CovaNell : ResistanceTransport
        {
            public CovaNell() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cova Nell",
                    4,
                    45,
                    isLimited: true,
                    abilityText: "While you defend or perform an attack, if you revealed a red maneuver roll 1 additional die.",
                    abilityType: typeof(CovaNellAbility)
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CovaNellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (HostShip.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.ColorComplexity == Movement.MovementComplexity.Complex)
            {
                Messages.ShowInfo("Cova Nell: +1 " + ((Combat.AttackStep == CombatStep.Attack) ? "attack" : "defense") + " die");
                count++;
            }
        }
    }
}
