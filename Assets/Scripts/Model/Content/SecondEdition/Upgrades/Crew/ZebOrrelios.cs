using Ship;
using Upgrade;
using UnityEngine;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class ZebOrrelios : GenericUpgrade
    {
        public ZebOrrelios() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Zeb\" Orrelios",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.ZebOrreliosCrewAbility),
                seImageNumber: 94
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ZebOrreliosCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.OnCanAttackBumpedTargetGlobal += CanAttack;
            GenericShip.OnUpdateWeaponRangeGlobal += AllowRange0Primaries;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCanAttackBumpedTargetGlobal -= CanAttack;
            GenericShip.OnUpdateWeaponRangeGlobal -= AllowRange0Primaries;
        }

        private void AllowRange0Primaries(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target)
        {
            if (weapon is PrimaryWeaponClass && (weapon.HostShip == HostShip || target == HostShip))
            {
                minRange = 0;
            }
        }

        private void CanAttack(ref bool canAttack, GenericShip attacker, GenericShip defender)
        {
            if (attacker.ShipsBumped.Contains(defender))
            {
                if (attacker.ShipId == HostShip.ShipId)
                {
                    ShotInfo shotInfo = new ShotInfo(attacker, defender, attacker.PrimaryWeapons);
                    if (shotInfo.InArc) canAttack = true;
                }
                else if (defender.ShipId == HostShip.ShipId)
                {
                    ShotInfo shotInfo = new ShotInfo(defender, attacker, defender.PrimaryWeapons);
                    if (shotInfo.InArc) canAttack = true;
                }
            }
        }

    }
}
