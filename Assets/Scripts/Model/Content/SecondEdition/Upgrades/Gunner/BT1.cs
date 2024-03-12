using Upgrade;
using SquadBuilderNS;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class BT1 : GenericUpgrade
    {
        public BT1() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "BT-1",
                UpgradeType.Gunner,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.BT1Ability),
                restriction: new FactionRestriction(Faction.Scum, Faction.Imperial),
                seImageNumber: 140
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(400, 1),
                new Vector2(150, 150)
            );
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            if (squadList.SquadFaction == Faction.Scum) return true;

            if (squadList.SquadFaction == Faction.Imperial)
            {
                if (squadList.HasUpgrade("Darth Vader") || squadList.HasPilot("Darth Vader"))
                {
                    return true;
                }
                else
                {
                    Messages.ShowError("0-0-0 cannot be in an Imperial squad without Darth Vader");
                    return false;
                }
            }

            return false;
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