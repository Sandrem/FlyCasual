using Bombs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class PaigeTico : Mg100StarFortress
        {
            public PaigeTico() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Paige Tico",
                    5,
                    60,
                    isLimited: true,
                    charges: 1,
                    regensCharges: true,
                    abilityType: typeof(Abilities.SecondEdition.PaigeTicoPilotAbility),
                    extraUpgradeIcon: Upgrade.UpgradeType.Talent
                );

                ModelInfo.SkinName = "Cobalt";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/48/7e/487e95ce-b13c-4d39-97f6-8e28e633d07f/swz66_paige-tico.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you drop a device, you may spend 1 charge to drop an additional device
    public class PaigeTicoPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBombWasDropped += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWasDropped -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWasDropped, AskToDropDevice);
        }

        private bool HasAvailableDevice()
        {
            return HostShip.UpgradeBar
                .GetInstalledUpgrades(Upgrade.UpgradeType.Device)
                .Any(device => device.State.Charges > 0);
        }

        private void AskToDropDevice(object sender, EventArgs e)
        {
            if (HostShip.State.Charges > 0 && HasAvailableDevice())
            {
                AskToUseAbility(
                    HostName,
                    NeverUseByDefault,
                    DropBomb,
                    descriptionLong: "Do you want to drop an additional device?",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void DropBomb(object sender, EventArgs e)
        {
            if (HostShip.State.Charges > 0 && HasAvailableDevice())
            {
                HostShip.State.Charges--;
                BombsManager.RegisterBombDropTriggerIfAvailable(
                    HostShip,
                    TriggerTypes.OnAbilityDirect,
                    onlyDrop: false,
                    isRealDrop: false
                );

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
