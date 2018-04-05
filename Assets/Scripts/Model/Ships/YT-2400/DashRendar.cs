using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace YT2400
    {
        public class DashRendar : YT2400
        {
            public DashRendar() : base()
            {
                PilotName = "Dash Rendar";
                PilotSkill = 7;
                Cost = 36;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.DashRendarAbility());
            }
        }
    }
}

namespace Abilities
{
    public class DashRendarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnActivationPhaseStart += ActivateDashRendarAbility;
            Phases.OnCombatPhaseStart_NoTriggers += DeactivateDashRendarAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnActivationPhaseStart -= ActivateDashRendarAbility;
            Phases.OnCombatPhaseStart_NoTriggers -= DeactivateDashRendarAbility;
        }

        private void ActivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = true;
        }

        private void DeactivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = false;
        }

    }
}
