using ActionsList;
using Arcs;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Enduring : GenericUpgrade
    {
        public Enduring() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Enduring",
                UpgradeType.Talent,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.EnduringAbility)
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/upgrades/enduring.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class EnduringAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst += CancelCritsFirstIfDefender;
            HostShip.OnDamageWasSuccessfullyDealt += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst -= CancelCritsFirstIfDefender;
            HostShip.OnDamageWasSuccessfullyDealt -= RegisterAbility;
        }

        private void CancelCritsFirstIfDefender(GenericShip ship)
        {
            if (!HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.Bullseye) && ship.ShipId == Combat.Defender.ShipId)
            {
                Combat.DiceRollAttack.CancelCritsFirst = true;
            }
        }

        private void RegisterAbility(GenericShip ship, bool flag)
        {
            if (flag)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageWasSuccessfullyDealt, PerformAction);
            }
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            List<GenericAction> actions = new List<GenericAction>();
            if (HostShip.ActionBar.HasAction(typeof(CalculateAction))) { actions.Add(new CalculateAction() { Color = Actions.ActionColor.Red }); }
            if (HostShip.ActionBar.HasAction(typeof(FocusAction))) { actions.Add(new FocusAction() { Color = Actions.ActionColor.Red }); }
            actions.ForEach(n => n.CanBePerformedWhileStressed = true);

            Messages.ShowInfoToHuman(HostName + ": After you suffer Critical damage, you may perform a Calculate or Focus action on your action bar, even while stressed, treating that action as red.");

            HostShip.AskPerformFreeAction(
                actions,
                delegate
                {
                    Selection.ThisShip = previousSelectedShip;
                    Triggers.FinishTrigger();
                },
                HostUpgrade.UpgradeInfo.Name,
                "After you suffer Critical damage, you may perform a Calculate or Focus action on your action bar, even while stressed, treating that action as red.",
                HostUpgrade
            );
        }
    }
}