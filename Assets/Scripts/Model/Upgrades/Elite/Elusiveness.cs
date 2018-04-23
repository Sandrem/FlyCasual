using Upgrade;
using UnityEngine;

namespace UpgradesList
{
    public class Elusiveness : GenericUpgrade
    {
        public Elusiveness() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Elusiveness";
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
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }
        
        public override int GetActionEffectPriority()
        {
            int result = 0;

            int potentialEvades = 0;
            if (Host.Tokens.HasToken(typeof(Tokens.EvadeToken))) potentialEvades++;
            int potentialDiceEvadeResults = (Host.Tokens.HasToken(typeof(Tokens.FocusToken))) ? 5 : 3;
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
            Host.Tokens.AssignToken(new Tokens.StressToken(Host), callBack);
        }

    }

}


