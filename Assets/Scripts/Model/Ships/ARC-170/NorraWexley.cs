using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using RuleSets;

namespace Ship
{
    namespace ARC170
    {
        public class NorraWexley : ARC170, ISecondEditionPilot
        {
            public NorraWexley() : base()
            {
                PilotName = "Norra Wexley";
                PilotSkill = 7;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.NorraWexleyPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 55;

                PilotAbilities.RemoveAll(ability => ability is Abilities.NorraWexleyPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.NorraWexleyPilotAbilitySE());

                SEImageNumber = 65;
            }
        }
    }
}

namespace Abilities
{
    public class NorraWexleyPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddNorraWexleyPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddNorraWexleyPilotAbility;
        }

        private void AddNorraWexleyPilotAbility(GenericShip ship)
        {
            NorraWexleyAction newAction = new NorraWexleyAction() { Host = this.HostShip };
            ship.AddAvailableDiceModification(newAction);
        }

        private class NorraWexleyAction : ActionsList.GenericAction
        {
            public NorraWexleyAction()
            {
                Name = DiceModificationName = "Norra Wexley's ability";

                TokensSpend.Add(typeof(Tokens.BlueTargetLockToken));
            }

            public override void ActionEffect(System.Action callBack)
            {
                char targetLockPair = GetTargetLockTokenLetterOnAnotherShip();
                if (targetLockPair != ' ')
                {
                    Combat.CurrentDiceRoll.AddDice(DieSide.Focus).ShowWithoutRoll();
                    Combat.CurrentDiceRoll.OrganizeDicePositions();

                    Host.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, targetLockPair);
                }
                else
                {
                    callBack();
                }
            }

            private char GetTargetLockTokenLetterOnAnotherShip()
            {
                GenericShip anotherShip = null;

                switch (Combat.AttackStep)
                {
                    case CombatStep.Attack:
                        anotherShip = Combat.Defender;
                        break;
                    case CombatStep.Defence:
                        anotherShip = Combat.Attacker;
                        break;
                    default:
                        break;
                }

                return Actions.GetTargetLocksLetterPair(Host, anotherShip);
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (GetTargetLockTokenLetterOnAnotherShip() != ' ') result = true;
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (GetTargetLockTokenLetterOnAnotherShip() != ' ')
                {
                    if (Host.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                    {
                        switch (Combat.AttackStep)
                        {
                            case CombatStep.Attack:
                                if (Host.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                                {
                                    result = 110;
                                }
                                break;
                            case CombatStep.Defence:
                                if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes + Combat.DiceRollDefence.Focuses)
                                { 
                                    result = 110;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                return result;
            }
        }

    }
}

namespace Abilities.SecondEdition
{
    public class NorraWexleyPilotAbilitySE : NorraWexleyPilotAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += ModifyDice;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= ModifyDice;
        }

        private void ModifyDice(DiceRoll roll)
        {
            int enemyShipsAtRangeOne = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Enemy).Count;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Defender == HostShip && enemyShipsAtRangeOne > 0)
            {
                Messages.ShowInfo("Norra Wexley: add evade dice enemy range 1.");
                roll.AddDice(DieSide.Success).ShowWithoutRoll();
                roll.OrganizeDicePositions();
            }
        }
    }
}