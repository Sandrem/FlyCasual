using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using Bombs;
using RuleSets;

namespace Ship
{
    namespace Firespray31
    {
        public class KoshkaFrost : Firespray31, ISecondEditionPilot
        {
            public KoshkaFrost() : base()
            {
                PilotName = "Koshka Frost";
                PilotSkill = 3;
                Cost = 71;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.KoshkaFrostAbility());

                faction = Faction.Scum;

                SkinName = "Boba Fett";
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KoshkaFrostAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Koshka Frost",
                IsAvailable,
                GetPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return ((Combat.AttackStep == CombatStep.Attack && Combat.Defender.IsStressed) ||
               (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.IsStressed));
        }

        private int GetPriority()
        {
            return 90;
        }

    }
}