using Ship;
using Upgrade;
using ActionsList;
using Actions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class CadBane : GenericUpgrade
    {
        public CadBane() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cad Bane",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.CadBaneCrewAbility),
                seImageNumber: 130
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(386, 0),
                new Vector2(200, 200)
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
            HostShip.AskPerformFreeAction(
                new BoostAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                "After you drop or launch a device, you may perform a red Boost action",
                HostUpgrade
            );
        }
    }
}