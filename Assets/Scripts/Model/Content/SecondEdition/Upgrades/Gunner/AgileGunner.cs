using Upgrade;
using Ship;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class AgileGunner : GenericUpgrade
    {
        public AgileGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Agile Gunner",
                UpgradeType.Gunner,
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.AgileGunnerAbility),
                seImageNumber: 162
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AgileGunnerAbility : GenericAbility
    {
        // During the End Phase, you may rotate your turret indicator.

        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": You may rotate your turret arc");

            /*HostShip.AskPerformFreeAction(new RotateArcAction() { IsRed = false, CanBePerformedWhileStressed = true }, Triggers.FinishTrigger);*/
            new RotateArcAction().DoOnlyEffect(Triggers.FinishTrigger);
        }
    }
}