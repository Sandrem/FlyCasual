using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class KeyanFarlander : BWing
        {
            public KeyanFarlander() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Keyan Farlander",
                    7,
                    29,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KeyanFarlanderAbiliity),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            ship.AddAvailableDiceModification(new KeyanFarlanderAction() { HostShip = HostShip });
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
                HostShip.Tokens.RemoveToken(
                    typeof(Tokens.StressToken),
                    callBack
                );
            }

            public override bool IsDiceModificationAvailable()
            {
                return HostShip.Tokens.HasToken(typeof(Tokens.StressToken)) && Combat.AttackStep == CombatStep.Attack;
            }

            public override int GetDiceModificationPriority()
            {
                if (HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    return 51;
                }
                return 0;
            }
        }
    }
}
