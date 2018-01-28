using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;

namespace Ship
{
    namespace BWing
    {
        public class KeyanFarlander : BWing
        {
            public KeyanFarlander() : base()
            {
                PilotName = "Keyan Farlander";
                PilotSkill = 7;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.KeyanFarlanderAbiliity());

                SkinName = "Blue";
            }
        }
    }
}

namespace Abilities
{
    public class KeyanFarlanderAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddKeyanFarlanderAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddKeyanFarlanderAbility;
        }

        private void AddKeyanFarlanderAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new KeyanFarlanderAction() { Host = HostShip });
        }

        private class KeyanFarlanderAction : ActionsList.GenericAction
        {
            public KeyanFarlanderAction()
            {
                Name = EffectName = "Keyan Farlander's ability";
                IsTurnsAllFocusIntoSuccess = true;
            }

            public override void ActionEffect(Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Success);
                Host.RemoveToken(
                    typeof(Tokens.StressToken),
                    callBack
                );
            }

            public override bool IsActionEffectAvailable()
            {
                return Host.HasToken(typeof(Tokens.StressToken)) && Combat.AttackStep == CombatStep.Attack;
            }

            public override int GetActionEffectPriority()
            {
                if (Host.HasToken(typeof(Tokens.StressToken)))
                {
                    return 51;
                }
                return 0;
            }
        }
    }
}
