using Arcs;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Dauntless : GenericUpgrade
    {
        public Dauntless() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dauntless",
                UpgradeType.Title,
                cost: 4,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.VT49Decimator.VT49Decimator)),
                abilityType: typeof(Abilities.SecondEdition.DauntlessAbility),
                seImageNumber: 123
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class DauntlessAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishUnsuccessfully += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishUnsuccessfully += RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToPerformWhiteActionAsRed);
        }

        private void AskToPerformWhiteActionAsRed(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You may perform an action");

            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActionsWhiteOnlyAsRed(),
                Triggers.FinishTrigger
            );
        }
    }
}