using Abilities;
using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class K4SecurityDroid : GenericUpgrade
    {
        public K4SecurityDroid() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "K4 Security Droid";
            Cost = 3;

            AvatarOffset = new Vector2(65, 5);

            UpgradeAbilities.Add(new K4SecurityDroidAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities
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
            Movement.ManeuverColor movementColor = HostShip.GetLastManeuverColor();
            if (movementColor != Movement.ManeuverColor.Green) return;

            if (Board.BoardManager.IsOffTheBoard(hostShip)) return;

            RegisterAbilityTrigger(
                TriggerTypes.OnShipMovementFinish,
                AssignK4TargetingLock
            );
        }

        private void AssignK4TargetingLock(object sender, EventArgs e)
        {
            Sounds.PlayShipSound("Astromech-Beeping-and-whistling");

            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                HostUpgrade.Name,
                HostUpgrade.ImageUrl
            );
        }
    }
}
