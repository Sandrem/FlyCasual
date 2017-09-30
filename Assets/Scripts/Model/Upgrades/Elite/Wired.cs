﻿using Upgrade;

namespace UpgradesList
{
    public class Wired : GenericUpgrade
    {
        public Wired() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Wired";
            Cost = 1;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += WiredActionEffect;
        }

        private void WiredActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.WiredActionEffect()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class WiredActionEffect : GenericAction
    {

        public WiredActionEffect()
        {
            Name = EffectName = "Wired";            
        }
        
        public override int GetActionEffectPriority()
        {
            int result = 0;
                        
            if (Combat.AttackStep == CombatStep.Defence && Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes && Combat.DiceRollDefence.Focuses > 0)
            {
                result = 30;
            }

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.Focuses > 0)
            {
                result = 30;                    
            }
            
            return result;            
        }

        public override bool IsActionEffectAvailable()
        {
            return Host.HasToken(typeof(Tokens.StressToken));
        }
        
        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = new System.Collections.Generic.List<DieSide> { DieSide.Focus },
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}


