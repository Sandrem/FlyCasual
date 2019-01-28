using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using System;

namespace UpgradesList.SecondEdition
{
    public class EmperorPalpatine : GenericUpgrade
    {
        public EmperorPalpatine() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Emperor Palpatine",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 11,
                isLimited: true,
                addForce: 1,                
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.EmperorPalpatineCrewAbility),
                seImageNumber: 115
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EmperorPalpatineCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                () => 20,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            GenericShip activeShip = Combat.AttackStep == CombatStep.Attack ? Combat.Attacker : Combat.Defender;
            return activeShip.Owner == HostShip.Owner
                && activeShip != HostShip                
                && HostShip.State.Force > 0;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.Force--;
                callback(true);
            }
            else
            {
                callback(false);
            }
        }
    }
}