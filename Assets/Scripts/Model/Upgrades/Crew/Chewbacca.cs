using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Chewbacca : GenericUpgrade
    {
        public Chewbacca() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Chewbacca";
            Cost = 4;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebels;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            Host.OnDamageCardIsDealt += RegisterChewbaccaCrewTrigger;
        }

        private void RegisterChewbaccaCrewTrigger(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Chewbacca's ability",
                TriggerType = TriggerTypes.OnDamageCardIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = AskUseChewbaccaCrewAbility,
                Sender = ship
            });
        }

        public override void Discard()
        {
            Host.OnDamageCardIsDealt -= RegisterChewbaccaCrewTrigger;
            base.Discard();
        }

        private void AskUseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            Phases.StartTemporarySubPhase(
                "Ability of Chewbacca (crew)",
                typeof(SubPhases.ChewbaccaCrewDecisionSubPhase),
                delegate
                {
                    Selection.ActiveShip = previousShip;
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}

namespace SubPhases
{

    public class ChewbaccaCrewDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Use ability of Chewbacca (crew)?";

            AddDecision("Yes", UseChewbaccaCrewAbility);
            AddDecision("No", DontUseChewbaccaCrewAbility);

            defaultDecision = "Yes";
        }

        private void UseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Chewbacca");
            Messages.ShowInfo("Chewbacca (crew) is used");
            Combat.CurrentCriticalHitCard = null;
            if (Selection.ActiveShip.TryRegenShields()) Messages.ShowInfo("Shield is restored");
            Selection.ActiveShip.UpgradeBar.GetInstalledUpgrades().Find(n => n.GetType() == typeof(UpgradesList.Chewbacca)).Discard();
            ConfirmDecision();
        }

        private void DontUseChewbaccaCrewAbility(object sender, System.EventArgs e)
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
