using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;
using Tokens;

namespace Ship
{
    namespace AttackShuttle
    {
        public class EzraBridger : AttackShuttle, ISecondEditionPilot
        {
            public EzraBridger() : base()
            {
                PilotName = "Ezra Bridger";
                PilotSkill = 4;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.EzraBridgerPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 41;
                MaxForce = 1;

                PilotAbilities.RemoveAll(ability => ability is Abilities.EzraBridgerPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.EzraBridgerPilotAbilitySE());
            }
        }
    }
}

namespace Abilities
{
    public class EzraBridgerPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddEzraBridgerPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddEzraBridgerPilotAbility;
        }


        private void AddEzraBridgerPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new EzraBridgerAction());
        }

        private class EzraBridgerAction : ActionsList.GenericAction
        {
            public EzraBridgerAction()
            {
                Name = DiceModificationName = "Ezra Bridger's ability";

                IsTurnsAllFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    result = true;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.Tokens.HasToken(typeof(Tokens.StressToken)))
                {
                    if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                    {
                        if (Combat.DiceRollDefence.Focuses > 0) result = 80;
                    }
                }

                return result;
            }
        }

    }
}

namespace Abilities.SecondEdition
{
    public class EzraBridgerPilotAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddEzraBridgerPilotAbilitySE;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddEzraBridgerPilotAbilitySE;
        }

        private void AddEzraBridgerPilotAbilitySE(GenericShip ship)
        {
            EzraBridgerActionSE newAction = new EzraBridgerActionSE() { Host = this.HostShip };
            ship.AddAvailableDiceModification(newAction);
        }

        private class EzraBridgerActionSE : ActionsList.GenericAction
        {
            public EzraBridgerActionSE()
            {
                Name = DiceModificationName = "Ezra Bridger's ability";

                TokensSpend.Add(typeof(Tokens.ForceToken));
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.Attacker.Force--;
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Host.Tokens.HasToken(typeof(Tokens.StressToken)) && Host.Tokens.HasToken(typeof(Tokens.ForceToken)))
                    return true;

                return false;
            }

            public override int GetDiceModificationPriority()
            {
                if (Combat.CurrentDiceRoll.Focuses > 0)
                {
                    if (Combat.CurrentDiceRoll.Focuses > 1)
                        return 50;
                    else
                        return 45;
                }

                return 0;
            }
        }

    }
}