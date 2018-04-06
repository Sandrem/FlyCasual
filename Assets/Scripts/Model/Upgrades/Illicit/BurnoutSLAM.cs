using Ship;
using Upgrade;
using Abilities;

namespace UpgradesList
{ 
    public class BurnoutSLAM : GenericUpgrade
    {
        public BurnoutSLAM() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Burnout SLAM";
            Cost = 1;

            UpgradeAbilities.Add(new BurnoutSlamAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Large;
        }
    }
}

namespace Abilities
{
    public class BurnoutSlamAbility : GenericActionBarAbility<ActionsList.SlamAction>
    {
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            HostShip.OnActionIsPerformed += RegisterBurnoutSlamAbility;
        }

        public override void DeactivateAbility()
        {
            base.DeactivateAbility();
            HostShip.OnActionIsPerformed -= RegisterBurnoutSlamAbility;
        }

        private void RegisterBurnoutSlamAbility(ActionsList.GenericAction action)
        {
            if (action != null && action.GetType() == typeof(ActionsList.SlamAction))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, DiscardThisUpgrade);
            }
        }

        private void DiscardThisUpgrade(object sender, System.EventArgs e)
        {
            HostUpgrade.TryDiscard(Triggers.FinishTrigger);
        }
    }
}
