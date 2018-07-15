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
            HostShip.OnGenerateDiceModifications += AddKeyanFarlanderAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddKeyanFarlanderAbility;
        }

        private void AddKeyanFarlanderAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new KeyanFarlanderAction() { Host = HostShip });
        }

        private class KeyanFarlanderAction : ActionsList.GenericAction
        {
            public KeyanFarlanderAction()
            {
                Name = DiceModificationName = "Keyan Farlander's ability";
                IsTurnsAllFocusIntoSuccess = true;
            }

            public override void ActionEffect(Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus, DieSide.Success);
                Host.Tokens.RemoveToken(
                    typeof(Tokens.StressToken),
                    callBack
                );
            }

            public override bool IsDiceModificationAvailable()
            {
                return Host.Tokens.HasToken(typeof(Tokens.StressToken)) && Combat.AttackStep == CombatStep.Attack;
            }

            public override int GetDiceModificationPriority()
            {
                if (Host.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    return 51;
                }
                return 0;
            }
        }
    }
}
