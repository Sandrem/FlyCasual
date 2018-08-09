using Upgrade;
using System.Linq;
using Abilities;
using Ship;
using ActionsList;

namespace UpgradesList
{
    public class Wired : GenericUpgrade
    {
        public Wired() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Wired";
            Cost = 1;

            UpgradeAbilities.Add(new WiredAbility());
        }
    }
}

namespace Abilities
{
    public class WiredAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += WiredActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= WiredActionEffect;
        }

        private void WiredActionEffect(GenericShip host)
        {
            GenericAction newAction = new WiredActionEffect()
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
    public class WiredActionEffect : GenericAction
    {

        public WiredActionEffect()
        {
            Name = DiceModificationName = "Wired";            
        }
        
        public override int GetDiceModificationPriority()
        {
            int result = 0;
                
            if (Combat.Attacker.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 95;
                }
                else
                {
                    result = 30;
                }
            }
            
            return result;            
        }

        public override bool IsDiceModificationAvailable()
        {
            return Host.Tokens.HasToken(typeof(Tokens.StressToken));
        }
        
        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = new System.Collections.Generic.List<DieSide> { DieSide.Focus },
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}


