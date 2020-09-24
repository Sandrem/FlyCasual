using Ship;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Abilities
{
    public abstract class CombinedAbility : TemplateAbility
    {
        public abstract List<Type> CombinedAbilities { get; }
        private List<GenericAbility> StoredAbilities { get; } = new List<GenericAbility>();

        public override void ActivateAbility()
        {
            if (StoredAbilities.Count == 0)
            {
                foreach (Type ability in CombinedAbilities)
                {
                    StoredAbilities.Add((GenericAbility)Activator.CreateInstance(ability));
                }
            }

            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.Initialize(Selection.ThisShip);
            }
        }

        public override void DeactivateAbility()
        {
            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.DeactivateAbility();
            }
        }

        public override void Initialize(GenericShip hostShip)
        {
            if (StoredAbilities.Count == 0)
            {
                foreach (Type ability in CombinedAbilities)
                {
                    StoredAbilities.Add((GenericAbility)Activator.CreateInstance(ability));
                }
            }

            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.Initialize(hostShip);
            }
        }

        public override void InitializeForSquadBuilder(GenericShip hostShip)
        {
            HostShip = HostUpgrade.HostShip;

            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.InitializeForSquadBuilder(hostShip);
            }
        }

        public override void InitializeForSquadBuilder(GenericUpgrade hostUpgrade)
        {
            HostUpgrade = hostUpgrade;
            HostShip = HostUpgrade.HostShip;

            if (StoredAbilities.Count == 0)
            {
                foreach (Type ability in CombinedAbilities)
                {
                    StoredAbilities.Add((GenericAbility)Activator.CreateInstance(ability));
                }
            }

            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.InitializeForSquadBuilder(hostUpgrade);
            }
        }
    }
}
