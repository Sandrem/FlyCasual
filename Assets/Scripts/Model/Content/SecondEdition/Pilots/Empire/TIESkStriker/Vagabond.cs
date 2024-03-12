﻿using Bombs;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class Vagabond : TIESkStriker
        {
            public Vagabond() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Vagabond\"",
                    "Destitute Demolitionist",
                    Faction.Imperial,
                    2,
                    4,
                    12,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/98/a8/98a8c9cc-53b2-4b57-ac0b-96da7c064740/swz66_vagabond.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a maneuver using your Adaptive Ailerons, if you are not stressed, you may drop 1 device
    public class VagabondAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
    {
            if (HostShip.AssignedManeuver.GrantedBy == "Ailerons")
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToDropDevice);
            }
        }

        private bool HasAvailableDevice()
        {
            return HostShip.UpgradeBar
                .GetInstalledUpgrades(UpgradeType.Device)
                .Any(device => device.State.Charges > 0);
        }

        private void AskToDropDevice(object sender, EventArgs e)
        {
            if (!HostShip.IsStressed && HasAvailableDevice())
            {
                AskToUseAbility(
                    HostName,
                    NeverUseByDefault,
                    DropBomb,
                    descriptionLong: "Do you want to drop a device?",
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
            if (!HostShip.IsStressed && HasAvailableDevice())
            {
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