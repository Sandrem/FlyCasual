using Ship;
using Ship.YWing;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList
{
    public class ExtraMunitions : GenericUpgrade
    {
        private List<GenericUpgrade> upgradesWithOrdnanceToken = new List<GenericUpgrade>();
        private string ordnanceTokenMarker = " (EM)";

        public ExtraMunitions() : base()
        {
            Type = UpgradeType.Torpedo;
            Name = "Extra Munitions";
            Cost = 2;
            isLimited = true;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Phases.OnGameStart += SetOrdnanceTokens;
            Host.OnDiscardUpgrade += CheckOrdnanceToken;
        }

        private void SetOrdnanceTokens()
        {
            foreach (var upgrade in Host.UpgradeBar.GetInstalledUpgrades())
            {
                if (upgrade.Type == UpgradeType.Torpedo || upgrade.Type == UpgradeType.Missile || upgrade.Type == UpgradeType.Bomb)
                {
                    SetOrdnanceToken(upgrade);
                }
            }
        }

        private void SetOrdnanceToken(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceToken.Add(upgrade);
            if (upgrade.Name != Name) upgrade.Name += ordnanceTokenMarker;

            Roster.UpdateUpgradesPanel(Host, Host.InfoPanel);
        }

        public void RemoveOrdnanceToken(GenericUpgrade upgrade)
        {
            upgradesWithOrdnanceToken.Remove(CurrentUpgrade);

            CurrentUpgrade.Name = CurrentUpgrade.Name.Replace(ordnanceTokenMarker, "");
            Roster.UpdateUpgradesPanel(Host, Host.InfoPanel);
        }

        private void CheckOrdnanceToken()
        {
            if (upgradesWithOrdnanceToken.Contains(CurrentUpgrade))
            {
                Triggers.RegisterTrigger(new Trigger(){
                    Name = "Extra Munitions",
                    TriggerType = TriggerTypes.OnDiscard,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = AskExtraMunitionsDecision
                });
            }
        }

        private void AskExtraMunitionsDecision(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhase(
                "Extra Munitions",
                typeof(SubPhases.ExtraMunitionsDecisionSubphase),
                Triggers.FinishTrigger
            );
        }
    }
}

namespace SubPhases
{

    public class ExtraMunitionsDecisionSubphase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Spend Ordnance token?";

            AddDecision("Yes", UseAbility);
            AddDecision("No", DontUseAbility);

            defaultDecision = "Yes";
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Ordnance token is discarded instead of " + GenericUpgrade.CurrentUpgrade.Name);

            UpgradesList.ExtraMunitions extraMunitions = (GenericUpgrade.CurrentUpgrade.Host.UpgradeBar.GetInstalledUpgrades().Find(n => n.GetType() == typeof(UpgradesList.ExtraMunitions)) as UpgradesList.ExtraMunitions);
            extraMunitions.RemoveOrdnanceToken(GenericUpgrade.CurrentUpgrade);
            
            GenericUpgrade.CurrentUpgrade = null;

            ConfirmDecision();
        }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
