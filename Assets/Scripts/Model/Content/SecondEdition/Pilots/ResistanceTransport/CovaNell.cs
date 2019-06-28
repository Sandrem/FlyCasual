﻿using Abilities.SecondEdition;
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
                    38,
                    isLimited: true,
                    abilityText: "While you defend or perform an attack, if you revealed a red maneuver roll 1 additional die.",
                    abilityType: typeof(CovaNellAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/1b/93/1b934d61-0f90-42d0-bf84-0052960b105b/swz45_cova-nell.png";
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
