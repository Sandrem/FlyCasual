using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Phases.OnActivationPhaseStart += ActivateDashRendarAbility;
            Phases.OnCombatPhaseStart += DeactivateDashRendarAbility;
            HostShip.OnDestroyed += DeactivateDashRendarAbilityOnDestroyed;
        }

        private void ActivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = true;
        }

        private void DeactivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = false;
        }

        private void DeactivateDashRendarAbilityOnDestroyed(GenericShip host)
        {
            Phases.OnActivationPhaseStart -= ActivateDashRendarAbility;
        }
    }
}
