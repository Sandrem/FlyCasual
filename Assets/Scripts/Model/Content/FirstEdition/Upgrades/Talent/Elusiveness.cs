using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class Elusiveness : GenericUpgrade
    {
        public Elusiveness() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Elusiveness",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.ElusivenessAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ElusivenessAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += ElusivenessActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= ElusivenessActionEffect;
        }

        private void ElusivenessActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ElusivenessActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class ElusivenessActionEffect : GenericAction
    {

        public ElusivenessActionEffect()
        {
            Name = DiceModificationName = "Elusiveness";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            int potentialEvades = 0;
            if (Host.Tokens.HasToken(typeof(Tokens.EvadeToken))) potentialEvades++;
            int potentialDiceEvadeResults = (Host.Tokens.HasToken(typeof(Tokens.FocusToken))) ? 5 : 3;
            float averageDefenceDiceResult = Host.State.Agility * (potentialDiceEvadeResults / 8);
            potentialEvades += Mathf.RoundToInt(averageDefenceDiceResult);
            if (Host.State.HullCurrent <= Host.State.HullMax / 2) potentialEvades--;

            if (Combat.DiceRollAttack.Successes > potentialEvades)
            {
                result = 80;
            }

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && !Host.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                IsOpposite = true,
                CallBack = delegate {
                    AssignStress(callBack);
                }
            };
            diceRerollManager.Start();
        }

        private void AssignStress(System.Action callBack)
        {
            Host.Tokens.AssignToken(typeof(StressToken), callBack);
        }

    }

}