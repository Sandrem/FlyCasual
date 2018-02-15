using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace Ship
{
    namespace BWing
    {
        public class Ibtisam : BWing
        {
            public Ibtisam() : base()
            {
                PilotName = "Ibtisam";
                PilotSkill = 6;
                Cost = 28;
                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
 
                PilotAbilities.Add(new Abilities.IbtisamAbiliity());
                SkinName = "Blue";
            }
        }
    }
}
 
namespace Abilities
{
    public class IbtisamAbiliity : GenericAbility
    {
        // When attacking or defending, if you have at least 1 stress token,
        // you may reroll 1 of your dice.
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddIbtisamAbility;
        }
 
        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddIbtisamAbility;
        }
 
        private void AddIbtisamAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new IbtisamAction() { Host = HostShip });
        }
 
        private class IbtisamAction : ActionsList.GenericAction
        {
            public IbtisamAction()
            {
                Name = EffectName = "Ibtisam's ability";
            }
 
            public override void ActionEffect(Action callBack)
            {
                // reroll one dice
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = 1,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }
 
            public override bool IsActionEffectAvailable()
            {
                return Host.Tokens.HasToken(typeof(Tokens.StressToken)) && (Combat.AttackStep == CombatStep.Attack || Combat.AttackStep == CombatStep.Defence);
            }
 
            public override int GetActionEffectPriority()
            {
                if (Host.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    return 90;
                }
                return 0;
            }
        }
    }
}
