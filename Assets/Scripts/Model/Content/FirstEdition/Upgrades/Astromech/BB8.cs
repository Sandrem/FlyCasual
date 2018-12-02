using System;
using Upgrade;
using Ship;
using UnityEngine;
using System.Collections.Generic;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class BB8 : GenericUpgrade
    {
        public BB8() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "BB-8",
                UpgradeType.Astromech,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.BB8Ability)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(85, 1));
        }
    }
}

namespace Abilities.FirstEdition
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
            if (host.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
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