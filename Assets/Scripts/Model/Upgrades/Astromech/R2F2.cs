using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;

namespace UpgradesList
{

    public class R2F2 : GenericUpgrade
    {
        public R2F2() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R2-F2";
            isUnique = true;
            Cost = 3;

            UpgradeAbilities.Add(new R2F2Ability());
        }
    }

}

namespace Abilities
{
    public class R2F2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += R2F2AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= R2F2AddAction;
        }

        private void R2F2AddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.R2F2Action()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableAction(action);
        }
    }
}

namespace ActionsList
{ 

    public class R2F2Action : GenericAction
    {
        public R2F2Action()
        {
            Name = EffectName = "R2-F2: Increase Agility";
        }

        public override void ActionTake()
        {
            Sounds.PlayShipSound("Astromech-Beeping-and-whistling");

            Host.ChangeAgilityBy(+1);
            Phases.OnEndPhaseStart_NoTriggers += R2F2DecreaseAgility;
            Host.Tokens.AssignCondition(new Conditions.R2F2Condition(Host));
            Phases.CurrentSubPhase.CallBack();
        }

        public override int GetActionPriority()
        {
            int result = 0;
            result = 10 * (Actions.CountEnemiesTargeting(Selection.ThisShip));
            return result;
        }

        private void R2F2DecreaseAgility()
        {
            Host.ChangeAgilityBy(-1);
            Host.Tokens.RemoveCondition(typeof(Conditions.R2F2Condition));
            Phases.OnEndPhaseStart_NoTriggers -= R2F2DecreaseAgility;
        }

    }

}

namespace Conditions
{

    public class R2F2Condition : Tokens.GenericToken
    {
        public R2F2Condition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new UpgradesList.R2F2().ImageUrl;
        }
    }

}
