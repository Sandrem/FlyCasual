using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class Ibtisam : BWing
        {
            public Ibtisam() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ibtisam",
                    6,
                    28,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.IbtisamAbiliity)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class IbtisamAbiliity : GenericAbility
    {
        // When attacking or defending, if you have at least 1 stress token,
        // you may reroll 1 of your dice.
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddIbtisamAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddIbtisamAbility;
        }

        private void AddIbtisamAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new IbtisamAction() { Host = HostShip });
        }

        private class IbtisamAction : ActionsList.GenericAction
        {
            public IbtisamAction()
            {
                Name = DiceModificationName = "Ibtisam's ability";
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

            public override bool IsDiceModificationAvailable()
            {
                return Host.Tokens.HasToken(typeof(StressToken)) && (Combat.AttackStep == CombatStep.Attack || Combat.AttackStep == CombatStep.Defence);
            }

            public override int GetDiceModificationPriority()
            {
                if (Host.Tokens.HasToken(typeof(StressToken)))
                {
                    return 90;
                }
                return 0;
            }
        }
    }
}
