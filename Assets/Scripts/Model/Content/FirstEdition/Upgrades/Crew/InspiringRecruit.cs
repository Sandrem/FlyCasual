﻿using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;
using System.Linq;
using System;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class InspiringRecruit : GenericUpgrade
    {
        public bool IsAbilityUsed;

        public InspiringRecruit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Inspiring Recruit",
                UpgradeType.Crew,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.InspiringRecruitAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class InspiringRecruitAbility : GenericAbility
    {
        private GenericShip ShipToRemoveStress;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsRemovedGlobal += CheckAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsRemovedGlobal -= CheckAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility(GenericShip ship, Type tokenType)
        {
            if (IsAbilityUsed) return;
            if (tokenType != typeof(StressToken)) return;
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, ship);
            if (distanceInfo.Range < 3)
            {
                ShipToRemoveStress = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskInspiringRecruitAbility);
            }
        }

        private void AskInspiringRecruitAbility(object sender, System.EventArgs e)
        {
            if (ShipToRemoveStress.Tokens.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(AlwaysUseByDefault, RemoveStress, null, null, true);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void RemoveStress(object sender, EventArgs e)
        {
            IsAbilityUsed = true;

            ShipToRemoveStress.Tokens.RemoveToken(
                typeof(StressToken),
                DecisionSubPhase.ConfirmDecision
            );
        }

    }
}