using ActionsList;

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
                    42,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.DeathrainAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 140;
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
