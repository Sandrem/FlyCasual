using System;
using Ship;
using UnityEngine;
using System.Linq;
using ActionsList;
using SubPhases;
using System.Collections.Generic;
using Upgrade;
using RuleSets;

namespace Upgrade
{
    public class GenericDualUpgrade : GenericUpgrade
    {
        protected Type AnotherSide { get; set; }
        private GenericDualUpgrade AnotherSideInstance { get; set; }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnShipIsPlaced += AskToSelectSide;
        }

        private void AskToSelectSide(GenericShip host)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select side of Dual Card",
                TriggerType = TriggerTypes.OnShipIsPlaced,
                TriggerOwner = host.Owner.PlayerNo,
                EventHandler = StartDecisionSubphase
            });
        }

        private void StartDecisionSubphase(object sender, EventArgs e)
        {
            DualCardSideDecisionSubphase decision = (DualCardSideDecisionSubphase) Phases.StartTemporarySubPhaseNew(
                "Select side of Dual Card",
                typeof(DualCardSideDecisionSubphase),
                Triggers.FinishTrigger
            );

            decision.Upgrade = this;
            decision.UpgradeTypes = new List<Type>() { this.GetType(), AnotherSide };

            decision.Start();
        }

        public void Flip(Action<GenericDualUpgrade> callback = null)
        {
            Messages.ShowInfo(string.Format("{0} was flipped", Name));
            Discard(() => SetAnotherSide(callback));
        }

        private void SetAnotherSide(Action<GenericDualUpgrade> callback = null)
        {
            if (AnotherSideInstance == null)
            {
                CreateAnotherSideInstance();
            }

            ReplaceUpgradeBy(AnotherSideInstance);
            if(callback != null)
            {
                callback(AnotherSideInstance);
            }
        }

        private void CreateAnotherSideInstance()
        {
            AnotherSideInstance = (GenericDualUpgrade) Activator.CreateInstance(AnotherSide);
            RuleSet.Instance.AdaptUpgradeToRules(AnotherSideInstance);
        }
    }

}

namespace SubPhases
{
    public class DualCardSideDecisionSubphase : DecisionSubPhase
    {
        public GenericDualUpgrade Upgrade;
        public List<Type> UpgradeTypes;

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Select side of Dual Card";

            foreach (var type in UpgradeTypes)
            {
                GenericDualUpgrade upgradeSide = (GenericDualUpgrade)Activator.CreateInstance(type);
                RuleSet.Instance.AdaptUpgradeToRules(upgradeSide);
                AddDecision(
                    upgradeSide.Name,
                    delegate { SelectSide(upgradeSide); },
                    upgradeSide.ImageUrl
                );
            }

            DefaultDecisionName = GetDecisions().First().Name;

            DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            DecisionOwner = Upgrade.Host.Owner;

            callBack();
        }

        private void SelectSide(GenericDualUpgrade newUpgradeSide)
        {
            if (Upgrade.GetType() != newUpgradeSide.GetType())
            {
                Upgrade.Flip((otherSide) => otherSide.Host.CallOnAfterDualUpgradeSideSelected(otherSide));
            }
            else
            {
                Upgrade.Host.CallOnAfterDualUpgradeSideSelected(Upgrade);
            }
            
            DecisionSubPhase.ConfirmDecision();
        }
    }
}
