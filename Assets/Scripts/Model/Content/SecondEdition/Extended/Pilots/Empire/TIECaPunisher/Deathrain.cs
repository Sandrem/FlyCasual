using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIECaPunisher
    {
        public class Deathrain : TIECaPunisher
        {
            public Deathrain() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Deathrain\"",
                    "Dexterous Bombardier",
                    Faction.Imperial,
                    4,
                    6,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DeathrainAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    seImageNumber: 140,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
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
