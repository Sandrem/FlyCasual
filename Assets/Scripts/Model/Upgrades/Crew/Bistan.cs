using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;

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

            AvatarOffset = new Vector2(43, 6);

            UpgradeAbilities.Add(new BistanAbility());
        }
 
        public override bool IsAllowedForShip(Ship.GenericShip ship)
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
            HostShip.AfterGenerateAvailableActionEffectsList += BistanAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= BistanAddAction;
        }

        private void BistanAddAction(GenericShip ship)
        {
            ActionsList.GenericAction action = new ActionsList.BistanAction()
            {
                Host = this.HostShip
            };
            ship.AddAvailableActionEffect(action);
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
            Name = EffectName = "Bistan";
        }
 
        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }
 
        public override bool IsActionEffectAvailable()
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

        public override int GetActionEffectPriority()
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
