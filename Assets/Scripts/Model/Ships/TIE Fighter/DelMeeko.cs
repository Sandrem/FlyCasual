using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Abilities;
using RuleSets;
using ActionsList;

namespace Ship
{
    namespace TIEFighter
    {
        public class DelMeeko : TIEFighter, ISecondEditionPilot
        {
            public DelMeeko() : base()
            {
                PilotName = "Del Meeko";
                PilotSkill = 4;
                Cost = 30;

                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new DelMeekoAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities
{
    // When another friendly ship at Range 2 is defending against a damaged ship, it may reroll 1 defense die.
    public class DelMeekoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddDelMeekoAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddDelMeekoAbility;
        }

        private void AddDelMeekoAbility(GenericShip ship)
        {
            Combat.Defender.AddAvailableDiceModification(new DelMeekoAction() { Host = this.HostShip });
        }

        private class DelMeekoAction : FriendlyRerollAction
        {
            public DelMeekoAction() : base(1, 2, true, RerollTypeEnum.DefenseDice)
            {
                Name = DiceModificationName = "Del Meeko's ability";
            }

            public override bool IsDiceModificationAvailable()
            {
                if (!Combat.Attacker.Damage.IsDamaged())
                    return false;
                else
                    return base.IsDiceModificationAvailable();
            }
        }
    }
}
