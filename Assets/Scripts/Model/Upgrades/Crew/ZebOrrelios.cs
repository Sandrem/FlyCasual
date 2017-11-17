using System;
using Upgrade;
using Ship;

namespace UpgradesList
{
    public class ZebOrrelios : GenericUpgrade
    {
        public ZebOrrelios() : base()
        {
            Type = UpgradeType.Crew;
            Name = "\"Zeb\" Orrelios";
            Cost = 1;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            GenericShip.OnCanAttackBumpedTargetGlobal += CanAttackIfInArc;
            Host.OnDestroyed += RemoveEffect;
        }

        private void CanAttackIfInArc(ref bool canAttack, GenericShip attacker, GenericShip defender)
        {
            if (attacker.ShipsBumped.Contains(defender))
            {
                if (attacker.ShipId == Host.ShipId)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(attacker, defender, attacker.PrimaryWeapon);
                    if (shotInfo.InArc) canAttack = true;
                }
                else if (defender.ShipId == Host.ShipId)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(defender, attacker, defender.PrimaryWeapon);
                    if (shotInfo.InArc) canAttack = true;
                }
            }
        }

        private void RemoveEffect(GenericShip ship)
        {
            GenericShip.OnCanAttackBumpedTargetGlobal -= CanAttackIfInArc;
        }

        public override void Discard(Action callBack)
        {
            GenericShip.OnCanAttackBumpedTargetGlobal -= CanAttackIfInArc;

            base.Discard(callBack);
        }
    }
}
