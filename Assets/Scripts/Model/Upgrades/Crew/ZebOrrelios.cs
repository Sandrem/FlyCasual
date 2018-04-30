using System;
using Upgrade;
using Ship;
using Abilities;
using UnityEngine;

namespace UpgradesList
{
    public class ZebOrrelios : GenericUpgrade
    {
        public ZebOrrelios() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "\"Zeb\" Orrelios";
            Cost = 1;

            isUnique = true;

            // AvatarOffset = new Vector2(54, 1);

            UpgradeAbilities.Add(new ZebOrreliosCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class ZebOrreliosCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.OnCanAttackBumpedTargetGlobal += CanAttackIfInArc;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCanAttackBumpedTargetGlobal -= CanAttackIfInArc;
        }


        private void CanAttackIfInArc(ref bool canAttack, GenericShip attacker, GenericShip defender)
        {
            if (attacker.ShipsBumped.Contains(defender))
            {
                if (attacker.ShipId == HostShip.ShipId)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(attacker, defender, attacker.PrimaryWeapon);
                    if (shotInfo.InArc) canAttack = true;
                }
                else if (defender.ShipId == HostShip.ShipId)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(defender, attacker, defender.PrimaryWeapon);
                    if (shotInfo.InArc) canAttack = true;
                }
            }
        }

    }
}
