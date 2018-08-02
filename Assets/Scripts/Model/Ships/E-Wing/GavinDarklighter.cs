using RuleSets;
using BoardTools;
using System.Collections.Generic;

namespace Ship
{
    namespace EWing
    {
        public class GavinDarklighter : EWing, ISecondEditionPilot
        {
            public GavinDarklighter() : base()
            {
                PilotName = "Gavin Darklighter";
                PilotSkill = 4;
                Cost = 68;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.GavinDarklighterAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship performs an attack, if the defender is in your arc, the attacker may change 1 hit result to a critical hit result.
    public class GavinDarklighterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                () => 20,
                DiceModificationType.Change,
                1, 
                new List<DieSide> { DieSide.Success },
                DieSide.Crit,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker.Owner == HostShip.Owner
                && new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapon).InPrimaryArc;
        }
    }
}
