using Ship;
using Upgrade;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class BurnoutSLAM : GenericUpgrade
    {
        public BurnoutSLAM() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Burnout SLAM",
                UpgradeType.Illicit,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.BurnoutSlamAbility),
                restrictionSize: BaseSize.Large
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class BurnoutSlamAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterBurnoutSlamAbility;
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new SlamAction(), HostUpgrade);
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterBurnoutSlamAbility;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(SlamAction), HostUpgrade);
        }

        private void RegisterBurnoutSlamAbility(GenericAction action)
        {
            if (action is SlamAction)
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