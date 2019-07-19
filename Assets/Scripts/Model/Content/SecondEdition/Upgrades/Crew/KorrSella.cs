﻿using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using Tokens;
using BoardTools;
using System;

namespace UpgradesList.SecondEdition
{
    public class KorrSella : GenericUpgrade
    {
        public KorrSella() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Korr Sella",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.KorrSellaAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/dd102b8e517399d12b17756a863d19ee.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KorrSellaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementExecuted += CheckAbility;   
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementExecuted -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver != null && ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnMovementExecuted,
                    ClearAllStressTokens,
                    isSkippable: true
                );
            }
        }

        private void ClearAllStressTokens(object sender, EventArgs e)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(StressToken)) > 0)
            {
                Messages.ShowInfo("Korr Sella: All stress tokens are removed");

                Selection.ThisShip.Tokens.RemoveAllTokensByType(
                    typeof(StressToken),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}