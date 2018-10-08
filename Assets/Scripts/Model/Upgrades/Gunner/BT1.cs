using Upgrade;
using Tokens;
using SquadBuilderNS;
using Abilities.SecondEdition;
using System.Collections.Generic;
using RuleSets;

namespace UpgradesList
{
    public class BT1 : GenericUpgrade, ISecondEditionUpgrade
    {
        public BT1() : base()
        {
            Types.Add(UpgradeType.Gunner);
            Name = "BT-1";
            Cost = 2;
            isUnique = true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            UpgradeAbilities.Add(new BT1Ability());

            SEImageNumber = 140;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Scum || ship.faction == Faction.Imperial;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = false;

            if (squadList.SquadFaction == Faction.Scum)
            {
                result = true;
            }

            if (squadList.SquadFaction == Faction.Imperial)
            {
                foreach (var shipHolder in squadList.GetShips())
                {
                    if (shipHolder.Instance.PilotName == "Darth Vader")
                    {
                        return true;
                    }

                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                    {
                        if (upgrade.Name == "Darth Vader")
                        {
                            return true;
                        }
                    }
                }

                if (result != true)
                {
                    Messages.ShowError("BT-1 cannot be in Imperial squad without Darth Vader");
                }
            }

            return result;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BT1Ability : GenericAbility
    {
        // While you perform an attack, you may change 1 (Hit) result to a (Crit) result for each stress token the defender has.

        public override void ActivateAbility()
        {
            AddDiceModification(
               HostUpgrade.Name,
               IsDiceModificationAvailable,
               GetDiceModificationAiPriority,
               DiceModificationType.Change,
               GetNumberOfDiceToModify,
               new List<DieSide>() { DieSide.Success },
               DieSide.Crit
           );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && Combat.Defender.Tokens.HasToken(typeof(StressToken)));
        }

        private int GetNumberOfDiceToModify()
        {
            return Combat.Defender.Tokens.CountTokensByType(typeof(StressToken));
        }

        private int GetDiceModificationAiPriority()
        {
            return 20;
        }

    }
}