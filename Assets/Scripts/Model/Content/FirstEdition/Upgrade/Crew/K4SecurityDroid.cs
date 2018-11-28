using Ship;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace UpgradesList.FirstEdition
{
    public class K4SecurityDroid : GenericUpgrade
    {
        public K4SecurityDroid() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "K4 Security Droid",
                UpgradeType.Crew,
                cost: 3,
                restrictionFaction: Faction.Scum,
                abilityType: typeof(Abilities.FirstEdition.K4SecurityDroidAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(65, 5));
        }        
    }
}

namespace Abilities.FirstEdition
{
    //Ability: After executing a green maneuver, you may acquire a target lock.
    public class K4SecurityDroidAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterK4SecurityDroid;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterK4SecurityDroid;
        }

        private void RegisterK4SecurityDroid(GenericShip hostShip)
        {
            Movement.MovementComplexity movementColor = HostShip.GetLastManeuverColor();
            if (movementColor != Movement.MovementComplexity.Easy) return;

            if (BoardTools.Board.IsOffTheBoard(hostShip)) return;

            RegisterAbilityTrigger(
                TriggerTypes.OnMovementFinish,
                AssignK4TargetingLock
            );
        }

        private void AssignK4TargetingLock(object sender, EventArgs e)
        {
            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                HostUpgrade
            );
        }
    }
}
