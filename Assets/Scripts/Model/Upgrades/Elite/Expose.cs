﻿using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class Expose : GenericUpgrade
    {
        public Expose() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Expose";
            Cost = 4;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += MarksmanshipAddAction;
        }

        private void MarksmanshipAddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ExposeAction();
            newAction.ImageUrl = ImageUrl;
            host.AddAvailableAction(newAction);
        }
    }
}

namespace ActionsList
{

    public class ExposeAction : GenericAction
    {

        public ExposeAction()
        {
            Name = EffectName = "Expose";
        }

        public override void ActionTake()
        {
            Host = Selection.ThisShip;
            ApplyExposeEffect();
        }

        private void ApplyExposeEffect()
        {
            Host.ChangeFirepowerBy(+1);
            Host.ChangeAgilityBy(-1);

            Phases.OnEndPhaseStart += RemoveExposeEffect;

            Host.AssignToken(new Conditions.ExposeCondition(), Phases.CurrentSubPhase.CallBack);
        }

        private void RemoveExposeEffect()
        {
            Host.ChangeFirepowerBy(-1);
            Host.ChangeAgilityBy(+1);

            Phases.OnEndPhaseStart -= RemoveExposeEffect;
        }

        public override int GetActionPriority()
        {
            int result = 0;
            if (Actions.HasTarget(Selection.ThisShip)) result = 60;
            return result;
        }

    }

}

namespace Conditions
{

    public class ExposeCondition : Tokens.GenericToken
    {
        public ExposeCondition()
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new UpgradesList.Expose().ImageUrl;
        }
    }

}
