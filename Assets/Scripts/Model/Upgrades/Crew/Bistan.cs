using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;

namespace UpgradesList
{
    
    public class Bistan : GenericUpgrade
    {
        public Bistan() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Bistan";
            Cost = 2;
            isUnique = true;

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(43, 6));

            UpgradeAbilities.Add(new BistanAbility());
        }
 
        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class BistanAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += BistanAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= BistanAddAction;
        }

        private void BistanAddAction(GenericShip ship)
        {
            GenericAction action = new BistanAction()
            {
                Host = this.HostShip,
                ImageUrl = HostUpgrade.ImageUrl
            };
            ship.AddAvailableDiceModification(action);
        }
    }
}
 
namespace ActionsList
{
    // When attacking at Range 1-2, you may change one successful hit  to one critical hit
    public class BistanAction : GenericAction
    {
        public BistanAction()
        {
            Name = DiceModificationName = "Bistan";
        }
 
        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }
 
        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                // use pre-calculated shot info through Combat.ShotInfo instead of calculating own
                // Board.ShipShotDistanceInformation
                if (Combat.ShotInfo.Range <= 2)
                {
                    result = true;
                }
            }
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 20;
            }

            return result;
        }
    }
}
