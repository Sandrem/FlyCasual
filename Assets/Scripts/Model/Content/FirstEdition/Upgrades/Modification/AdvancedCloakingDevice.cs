using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class AdvancedCloakingDevice : GenericUpgrade
    {
        public AdvancedCloakingDevice() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Cloaking Device",
                UpgradeType.Modification,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.AdvancedCloakingDeviceAbility),
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEPhantom.TIEPhantom))
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AdvancedCloakingDeviceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += RegisterPerformFreeCloakAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= RegisterPerformFreeCloakAction;
        }

        private void RegisterPerformFreeCloakAction(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Advanced Cloaking Device",
                TriggerType = TriggerTypes.OnAttackFinish,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = PerformFreeCloakAction
            });
        }

        private void PerformFreeCloakAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = new List<GenericAction>() { new CloakAction() };

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}