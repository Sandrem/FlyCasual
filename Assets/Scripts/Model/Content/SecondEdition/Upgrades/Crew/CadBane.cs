using Ship;
using Upgrade;
using ActionsList;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class CadBane : GenericUpgrade
    {
        public CadBane() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cad Bane",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.CadBaneCrewAbility),
                seImageNumber: 130
            );
        }        
    }
}


namespace Abilities.SecondEdition
{
    public class CadBaneCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBombWasDropped += RegisterDropTrigger;
            HostShip.OnBombWasLaunched += RegisterLaunchTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWasDropped -= RegisterDropTrigger;
            HostShip.OnBombWasLaunched -= RegisterLaunchTrigger;
        }

        private void RegisterDropTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWasDropped, AskToPerformBoost);
        }

        private void RegisterLaunchTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWasLaunched, AskToPerformBoost);
        }

        private void AskToPerformBoost(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You may perform a red Boost action");

            HostShip.AskPerformFreeAction(
                new BoostAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger
            );
        }
    }
}