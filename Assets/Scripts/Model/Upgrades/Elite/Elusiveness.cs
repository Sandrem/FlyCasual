using Upgrade;
using UnityEngine;
using System.Collections.Generic;

namespace UpgradesList
{
    public class Elusiveness : GenericUpgrade
    {
        public Elusiveness() : base()
        {
            Type = UpgradeType.Elite;
            Name = ShortName = "Elusiveness";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableOppositeActionEffectsList += ElusivenessActionEffect;
        }

        private void ElusivenessActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ElusivenessActionEffect()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableOppositeActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class ElusivenessActionEffect : GenericAction
    {

        public ElusivenessActionEffect()
        {
            Name = EffectName = "Elusiveness";
            IsOpposite = true;
        }
        
        public override int GetActionEffectPriority()
        {
            int result = 0;

            int potentialEvades = 0;
            if (Host.HasToken(typeof(Tokens.EvadeToken))) potentialEvades++;
            int potentialDiceEvadeResults = (Host.HasToken(typeof(Tokens.FocusToken))) ? 5 : 3;
            float averageDefenceDiceResult = Host.Agility * (potentialDiceEvadeResults/8);
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
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && !Host.HasToken(typeof(Tokens.StressToken)))
            {
                result = true;
            }

            return result;
        }
        
        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDicesCanBeRerolled = 1,
                IsOpposite = true,
                CallBack = delegate {
                    AssignStress(callBack);
                }
            };
            diceRerollManager.Start();
        }

        private void AssignStress(System.Action callBack)
        {
            Host.AssignToken(new Tokens.StressToken(), callBack);
        }

    }

}


