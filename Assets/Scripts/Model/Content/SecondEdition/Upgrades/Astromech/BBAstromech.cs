using System;
using Upgrade;
using Ship;
using UnityEngine;
using System.Collections.Generic;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class BBAstromech : GenericUpgrade
    {
        public BBAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "BB Astromech",
                UpgradeType.Astromech,
                charges: 2,
                cost: 5,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.BBAstromechAbility)
            );
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f8/fd/f8fd534a-43df-4285-a41c-1f8a789d06a5/swz25_bb-astromech_a1.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //Before you execute a blue maneuver, you may spend 1 charge to perform a barrel roll action.
    public class BBAstromechAbility : GenericAbility
    {
        protected List<GenericAction> AbilityActions = new List<GenericAction> { new BarrelRollAction() };

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += PlanAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= PlanAction;
        }

        private void PlanAction(GenericShip host)
        {
            if (host.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskPerformAction);
            }
        }

        private void AskPerformAction(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                AbilityActions,
                SpendCharge
            );
        }

        private void SpendCharge()
        {
            Sounds.PlayShipSound("BB-8-Sound");
            HostUpgrade.State.SpendCharge();
            Triggers.FinishTrigger();
        }
    }
}