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
            Debug.Log("Activate");

            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.ActivateAbility();
            }

            Debug.Log("Activated: " + StoredAbilities.Count);
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
            Debug.Log("Initialize");
            
            foreach (Type ability in CombinedAbilities)
            {
                StoredAbilities.Add((GenericAbility)Activator.CreateInstance(ability));
            }
            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.Initialize(hostShip);
            }

            Debug.Log("Initialized: " + StoredAbilities.Count);
        }

        public override void InitializeForSquadBuilder(GenericShip hostShip)
        {
            Debug.Log("Initialize SB");
            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.InitializeForSquadBuilder(hostShip);
            }
            Debug.Log("Initialized SB: " + StoredAbilities.Count);
        }

        public override void InitializeForSquadBuilder(GenericUpgrade hostUpgrade)
        {
            Debug.Log("Initialize SB");
            foreach (GenericAbility ability in StoredAbilities)
            {
                ability.InitializeForSquadBuilder(hostUpgrade);
            }
            Debug.Log("Initialized SB: " + StoredAbilities.Count);
        }
    }
}
