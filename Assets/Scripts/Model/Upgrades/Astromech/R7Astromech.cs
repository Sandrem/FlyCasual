using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{

    public class R7Astromech : GenericUpgrade
    {   
        public R7Astromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R7 Astromech";
            Cost = 2;

            UpgradeAbilities.Add(new R7AstromechAbility());
        }

    }

}

namespace Abilities
{
    public class R7AstromechAbility : GenericAbility
    {
        public bool usedThisRound;
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableOppositeActionEffectsList += CheckR7AstromechAbility;
            Phases.OnPlanningPhaseStart += RechargeAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableOppositeActionEffectsList -= CheckR7AstromechAbility;
            Phases.OnPlanningPhaseStart -= RechargeAbility;
        }

        private void CheckR7AstromechAbility(Ship.GenericShip ship)
        {
            if (!usedThisRound)
            {
                ActionsList.GenericAction newAction = new ActionsList.R7AstromechActionEffect(this)
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    Host = HostShip
                };
                HostShip.AddAvailableOppositeActionEffect(newAction);
            }
        }

        private void RechargeAbility()
        {
            usedThisRound = false;
        }
    }
}

namespace ActionsList
{
    public class R7AstromechActionEffect : GenericAction
    {
        R7AstromechAbility hostAbility;

        public R7AstromechActionEffect(R7AstromechAbility ability)
        {
            Name = EffectName = "R7 Astromech";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
            TokensSpend.Add(typeof(Tokens.BlueTargetLockToken));
            hostAbility = ability;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            int potentialEvades = 0;
            if (Host.Tokens.HasToken(typeof(Tokens.EvadeToken))) potentialEvades++;
            int potentialDiceEvadeResults = (Host.Tokens.HasToken(typeof(Tokens.FocusToken))) ? 5 : 3;
            float averageDefenceDiceResult = Host.Agility * (potentialDiceEvadeResults / 8);
            potentialEvades += Mathf.RoundToInt(averageDefenceDiceResult);
            if (Host.Hull <= Host.Hull / 2) potentialEvades--;

            if (Combat.DiceRollAttack.Successes > potentialEvades)
            {
                result = 80;
            }

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && Actions.HasTargetLockOn(Combat.Defender, Combat.Attacker) && !hostAbility.usedThisRound;            
        }

        public override void ActionEffect(System.Action callBack)
        {
            hostAbility.usedThisRound = true;
            SpendTargetLock(() =>
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = int.MaxValue,
                    IsOpposite = true,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            });            
        }

        private void SpendTargetLock(System.Action callBack)
        {
            var letter = Combat.Defender.Tokens.GetTargetLockLetterPair(Combat.Attacker);
            Host.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, letter);
        }

    }

}