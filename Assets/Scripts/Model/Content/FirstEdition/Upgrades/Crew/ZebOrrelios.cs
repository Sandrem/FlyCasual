using Ship;
using Upgrade;
using UnityEngine;
using BoardTools;

namespace UpgradesList.FirstEdition
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
                abilityType: typeof(Abilities.FirstEdition.ZebOrreliosCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(54, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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
