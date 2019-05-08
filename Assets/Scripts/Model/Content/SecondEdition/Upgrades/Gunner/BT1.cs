using Upgrade;
using SquadBuilderNS;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class BT1 : GenericUpgrade
    {
        public BT1() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Gunner,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.BT1Ability),
                restriction: new FactionRestriction(Faction.Scum, Faction.Imperial),
                seImageNumber: 140
            );
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
                    if (shipHolder.Instance.PilotInfo.PilotName == "Darth Vader")
                    {
                        return true;
                    }

                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                    {
                        if (upgrade.UpgradeInfo.Name == "Darth Vader")
                        {
                            return true;
                        }
                    }
                }

                if (result != true)
                {
                    Messages.ShowError("BT-1 cannot be in an Imperial squad without Darth Vader");
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
               HostUpgrade.UpgradeInfo.Name,
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