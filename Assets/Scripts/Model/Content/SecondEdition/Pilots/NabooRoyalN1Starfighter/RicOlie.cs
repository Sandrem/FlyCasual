﻿using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class RicOlie : NabooRoyalN1Starfighter
        {
            public RicOlie() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ric Olie",
                    5,
                    42,
                    isLimited: true,
                    abilityText: "When you defend or perform a primary attack, if the maneuver you revealed is greater than the enemy ship’s maneuver, roll 1 additional die.",
                    abilityType: typeof(RicOlieAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d2/7b/d27baa36-e835-4311-b86b-b9b19ce8a0b9/swz40_ric-olie.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RicOlieAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckAttackAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckDefensebility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckAttackAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckDefensebility;
        }

        private void CheckAttackAbility(ref int count)
        {
            if (HostShip.RevealedManeuver == null || Combat.Defender.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.Speed > Combat.Defender.RevealedManeuver.Speed)
            {
                Messages.ShowInfo("Ric Olie: +1 attack die");
                count++;
            }
        }

        private void CheckDefensebility(ref int count)
        {
            if (HostShip.RevealedManeuver == null || Combat.Attacker.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.Speed > Combat.Attacker.RevealedManeuver.Speed)
            {
                Messages.ShowInfo("Ric Olie: +1 defense die");
                count++;
            }
        }
    }
}
