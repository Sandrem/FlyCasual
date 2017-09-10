using Upgrade;

namespace UpgradesList
{
    public class Wired : GenericUpgrade
    {
        public Wired() : base()
        {
            Type = UpgradeType.Elite;
            Name = ShortName = "Wired";
            ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/e/e2/Wired-0.png";
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

            if (!Host.HasToken(typeof(Tokens.FocusToken)))
            {
                if (Combat.AttackStep == CombatStep.Defence && Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes && Combat.DiceRollDefence.Focuses > 0)
                {
                    result = 50;
                }

                if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 50;                    
                }
            }
            
            return result;            
        }

        public override bool IsActionEffectAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack || Combat.AttackStep == CombatStep.Defence) && Host.HasToken(typeof(Tokens.StressToken));
        }
        
        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = new System.Collections.Generic.List<DiceSide> { DiceSide.Focus },
                NumberOfDicesCanBeRerolled = 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}


