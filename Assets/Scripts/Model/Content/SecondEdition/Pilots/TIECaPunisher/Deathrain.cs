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
            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActions(),
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you drop or launch a device, you may perform an action",
                HostShip
            );
        }
    }
}
