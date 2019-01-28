using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIECaPunisher
    {
        public class Deathrain : TIECaPunisher
        {
            public Deathrain() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Deathrain\"",
                    4,
                    44,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DeathrainAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 140
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeathrainAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBombWasDropped += CheckAbilityOnDrop;
            HostShip.OnBombWasLaunched += CheckAbilityOnLaunch;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWasDropped -= CheckAbilityOnDrop;
            HostShip.OnBombWasLaunched -= CheckAbilityOnLaunch;
        }

        private void CheckAbilityOnDrop()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWasDropped, AskToPerformFreeAction);
        }

        private void CheckAbilityOnLaunch()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWasLaunched, AskToPerformFreeAction);
        }

        private void AskToPerformFreeAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("\"Deathrain\" can perform an action");

            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), Triggers.FinishTrigger);
        }
    }
}
