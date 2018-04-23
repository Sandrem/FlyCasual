using Ship;
using System;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Chewbacca : GenericUpgrade
    {
        public Chewbacca() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Chewbacca";
            Cost = 4;

            isUnique = true;

            AvatarOffset = new Vector2(66, 1);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
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

        public override void Discard(Action callBack)
        {
            Host.OnDamageCardIsDealt -= RegisterChewbaccaCrewTrigger;
            base.Discard(callBack);
        }

        private void AskUseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            GenericShip previousShip = Selection.ActiveShip;
            Selection.ActiveShip = sender as GenericShip;

            Phases.StartTemporarySubPhaseOld(
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

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Use ability of Chewbacca (crew)?";
            RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;

            AddDecision("Yes", UseChewbaccaCrewAbility);
            AddDecision("No", DontUseChewbaccaCrewAbility);

            DefaultDecisionName = "Yes";

            callBack();
        }

        private void UseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Chewbacca");
            Messages.ShowInfo("Chewbacca (crew) is used");
            Combat.CurrentCriticalHitCard = null;
            if (Selection.ActiveShip.TryRegenShields()) Messages.ShowInfo("Shield is restored");

            UpgradesList.Chewbacca chewbaccaUpgrade = Selection.ActiveShip.UpgradeBar.GetUpgradesOnlyFaceup().Find(n => n.GetType() == typeof(UpgradesList.Chewbacca)) as UpgradesList.Chewbacca;
            chewbaccaUpgrade.TryDiscard(ConfirmDecision);
        }

        private void DontUseChewbaccaCrewAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

    }

}
