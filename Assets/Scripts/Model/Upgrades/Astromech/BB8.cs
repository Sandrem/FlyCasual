using System;
using Upgrade;
using Abilities;
using Ship;
using System.Collections.Generic;
using ActionsList;
using UnityEngine;

namespace UpgradesList
{

    public class BB8 : GenericUpgrade
    {

        public BB8() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "BB-8";
            isUnique = true;
            Cost = 2;

            AvatarOffset = new Vector2(85, 1);

            UpgradeAbilities.Add(new BB8Ability());
        }
    }

}

namespace Abilities
{
    public class BB8Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += BB8PlanBarrelRoll;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= BB8PlanBarrelRoll;
        }

        private void BB8PlanBarrelRoll(GenericShip host)
        {
            if (host.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, BB8AskBarrelRoll);
            }
        }

        private void BB8AskBarrelRoll(object sender, EventArgs e)
        {
            Sounds.PlayShipSound("BB-8-Sound");

            HostShip.AskPerformFreeAction(
                new List<GenericAction>() { new BarrelRollAction() },
                Triggers.FinishTrigger
            );
        }
    }
}