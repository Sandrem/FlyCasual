using Upgrade;
using UnityEngine;
using Ship;

namespace UpgradesList
{
    public class Expose : GenericUpgrade
    {
        public Expose() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Expose";
            Cost = 4;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += ExposeAddAction;
        }

        private void ExposeAddAction(GenericShip host)
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

            Phases.OnEndPhaseStart_NoTriggers += RemoveExposeEffect;

            Host.Tokens.AssignCondition(new Conditions.ExposeCondition(Host));
            Phases.CurrentSubPhase.CallBack();
        }

        private void RemoveExposeEffect()
        {
            Host.ChangeFirepowerBy(-1);
            Host.ChangeAgilityBy(+1);

            Phases.OnEndPhaseStart_NoTriggers -= RemoveExposeEffect;
        }

        public override int GetActionPriority()
        {
            return 10;
        }

    }

}

namespace Conditions
{

    public class ExposeCondition : Tokens.GenericToken
    {
        public ExposeCondition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Tooltip = new UpgradesList.Expose().ImageUrl;
        }
    }

}
