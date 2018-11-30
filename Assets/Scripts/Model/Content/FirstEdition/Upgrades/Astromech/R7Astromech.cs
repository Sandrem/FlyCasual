using Upgrade;
using System.Linq;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class R7Astromech : GenericUpgrade
    {
        public R7Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R7 Astromech",
                UpgradeType.Astromech,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.R7AstromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
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
        Abilities.FirstEdition.R7AstromechAbility hostAbility;

        public R7AstromechActionEffect(Abilities.FirstEdition.R7AstromechAbility ability)
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
            float averageDefenceDiceResult = Host.State.Agility * (potentialDiceEvadeResults / 8);
            potentialEvades += UnityEngine.Mathf.RoundToInt(averageDefenceDiceResult);
            if (Host.State.HullCurrent <= Host.State.HullMax / 2) potentialEvades--;

            if (Combat.DiceRollAttack.Successes > potentialEvades)
            {
                result = 80;
            }

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && ActionsHolder.HasTargetLockOn(Combat.Defender, Combat.Attacker) && !hostAbility.usedThisRound;
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
            List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Defender, Combat.Attacker);
            Host.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, letters.First());
        }

    }

}