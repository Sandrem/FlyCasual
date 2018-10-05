using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class ProtonRockets : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public ProtonRockets() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Proton Rockets";
            Cost = 3;

            MinRange = 1;
            MaxRange = 1;
            AttackValue = 2;

            RequiresFocusToShoot = true;

            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new ProtonRocketsAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            IsDiscardedForShot = false;

            Cost = 7;
            UsesCharges = true;
            MaxCharges = 1;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 5;
            ArcRestrictions.Add(Arcs.ArcTypes.Bullseye);

            UpgradeAbilities.RemoveAll(a => a is ProtonRocketsAbility);

            SEImageNumber = 41;
        }
    }
}

namespace Abilities
{
    public class ProtonRocketsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckProtonRocketsAbility;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after attack dice are rolled
            HostShip.OnDefenceStartAsDefender += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned()
        {
            HostShip.OnDefenceStartAsDefender -= DeactivateAbilityPlanned;
            HostShip.AfterGotNumberOfAttackDice -= CheckProtonRocketsAbility;
        }

        private void CheckProtonRocketsAbility(ref int diceCount)
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                diceCount += Mathf.Min(Combat.Attacker.Agility, 3);
            }
        }
    }
}