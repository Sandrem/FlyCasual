﻿using Upgrade;
using UnityEngine;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class R2Astromech : GenericUpgrade, IVariableCost
    {
        public R2Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2 Astromech",
                UpgradeType.Astromech,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.R2AstromechAbility),
                charges: 2,
                seImageNumber: 53
            );
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> agilityToCost = new Dictionary<int, int>()
            {
                {0, 3},
                {1, 5},
                {2, 7},
                {3, 9}
            };

            UpgradeInfo.Cost = agilityToCost[ship.ShipInfo.Agility];
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you reveal your dial, you may spend 1 charge and gain 1 disarm token to recover 1 shield.
    public class R2AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += PlanRegenShield;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= PlanRegenShield;
        }

        private void PlanRegenShield(GenericShip host)
        {
            if (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                RegenShield,
                descriptionLong: "Do you want to spend 1 Charge and gain 1 Disarm Token to recover 1 shield?",
                imageHolder: HostUpgrade
            );
        }

        private void RegenShield(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), () =>
            {
                if (HostShip.TryRegenShields())
                {
                    Sounds.PlayShipSound("R2D2-Proud");
                    Messages.ShowInfo(HostName + " causes " + HostShip.PilotInfo.PilotName + " to recover 1 shield and gain 1 disarm token");
                }
                Triggers.FinishTrigger();
            });
        }
    }
}