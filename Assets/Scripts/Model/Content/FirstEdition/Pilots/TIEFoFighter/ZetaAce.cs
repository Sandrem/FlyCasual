﻿using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFoFighter
    {
        public class ZetaAce : TIEFoFighter
        {
            public ZetaAce() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeta Ace\"",
                    5,
                    18,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ZetaAceAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class ZetaAceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates += ChangeBarrelRollTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates -= ChangeBarrelRollTemplates;
        }

        private void ChangeBarrelRollTemplates(List<ActionsHolder.BarrelRollTemplates> availableTemplates)
        {
            availableTemplates.Add(ActionsHolder.BarrelRollTemplates.Straight2);
        }
    }
}