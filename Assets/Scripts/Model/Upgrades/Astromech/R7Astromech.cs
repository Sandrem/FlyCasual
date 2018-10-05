using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using System.Linq;

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
            HostShip.OnGenerateDiceModificationsOpposite += CheckR7AstromechAbility;
            Phases.Events.OnPlanningPhaseStart += RechargeAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= CheckR7AstromechAbility;
            Phases.Events.OnPlanningPhaseStart -= RechargeAbility;
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
                HostShip.AddAvailableDiceModification(newAction);
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
            Name = DiceModificationName = "R7 Astromech";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
            TokensSpend.Add(typeof(Tokens.BlueTargetLockToken));
            hostAbility = ability;
        }

        public override int GetDiceModificationPriority()
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

        public override bool IsDiceModificationAvailable()
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
            List<char> letters = Actions.GetTargetLocksLetterPairs(Combat.Defender, Combat.Attacker);
            Host.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, letters.First());
        }

    }

}