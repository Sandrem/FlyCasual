using Ship;
using Upgrade;
using System.Collections.Generic;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class Expose : GenericUpgrade
    {
        public Expose() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Expose",
                UpgradeType.Talent,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.ExposeAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ExposeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += ExposeAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= ExposeAddAction;
        }

        private void ExposeAddAction(GenericShip host)
        {
            GenericAction newAction = new ExposeAction
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = host
            };
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
            Name = DiceModificationName = "Expose";
        }

        public override void ActionTake()
        {
            HostShip = Selection.ThisShip;
            ApplyExposeEffect();
        }

        private void ApplyExposeEffect()
        {
            HostShip.ChangeFirepowerBy(+1);
            HostShip.ChangeAgilityBy(-1);

            Phases.Events.OnEndPhaseStart_NoTriggers += RemoveExposeEffect;

            HostShip.Tokens.AssignCondition(typeof(Conditions.ExposeCondition));
            Phases.CurrentSubPhase.CallBack();
        }

        private void RemoveExposeEffect()
        {
            HostShip.ChangeFirepowerBy(-1);
            HostShip.ChangeAgilityBy(+1);

            Phases.Events.OnEndPhaseStart_NoTriggers -= RemoveExposeEffect;
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
            Tooltip = new UpgradesList.FirstEdition.Expose().ImageUrl;
        }
    }

}