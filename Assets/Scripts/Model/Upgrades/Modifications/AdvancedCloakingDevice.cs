using Ship;
using Ship.TIEPhantom;
using Upgrade;
using ActionsList;
using System.Collections.Generic;
using Abilities;

namespace UpgradesList
{
    public class AdvancedCloakingDevice : GenericUpgrade
    {
        public AdvancedCloakingDevice() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Advanced Cloaking Device";
            Cost = 4;

            UpgradeAbilities.Add(new AdvancedCloakingDeviceAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEPhantom;
        }
    }
}

namespace Abilities
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

