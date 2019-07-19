﻿using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class Finn : ResistanceTransportPod
    {
        public Finn()
        {
            PilotInfo = new PilotCardInfo(
                "Finn",
                2,
                29,
                isLimited: true,
                abilityType: typeof(FinnResistanceTransportPodAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/311d88e51a039b79e9a422ab3c475288.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FinnResistanceTransportPodAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddFinnDiceModifications;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddFinnDiceModifications;
        }

        private void AddFinnDiceModifications(GenericShip ship)
        {
            ship.AddAvailableDiceModification(
                new FinnTransportPodDiceModificationBlank() {
                    HostShip = HostShip,
                    ImageUrl = HostShip.ImageUrl
                }
            );

            ship.AddAvailableDiceModification(
                new FinnTransportPodDiceModificationFocus() {
                    HostShip = HostShip,
                    ImageUrl = HostShip.ImageUrl
                }
            );
        }
    }
}

namespace ActionsList
{
    public class FinnTransportPodDiceModificationBlank : GenericAction
    {
        public FinnTransportPodDiceModificationBlank()
        {
            Name = DiceModificationName = "Finn (Blank)";
        }

        public override int GetDiceModificationPriority()
        {
            return 0;
        }

        public override bool IsDiceModificationAvailable()
        {
            return true;
        }

        public override void ActionEffect(Action callBack)
        {
            HostShip.AddAlreadyUsedDiceModification(new FinnTransportPodDiceModificationFocus() { HostShip = HostShip });

            Combat.CurrentDiceRoll.AddDice(DieSide.Blank).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            callBack();
        }
    }

    public class FinnTransportPodDiceModificationFocus : GenericAction
    {
        public FinnTransportPodDiceModificationFocus()
        {
            Name = DiceModificationName = "Finn (Focus)";
        }

        public override int GetDiceModificationPriority()
        {
            return 0;
        }

        public override bool IsDiceModificationAvailable()
        {
            return true;
        }

        public override void ActionEffect(Action callBack)
        {
            HostShip.AddAlreadyUsedDiceModification(new FinnTransportPodDiceModificationBlank() { HostShip = HostShip });

            Combat.CurrentDiceRoll.AddDice(DieSide.Focus).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), callBack);
        }
    }
}