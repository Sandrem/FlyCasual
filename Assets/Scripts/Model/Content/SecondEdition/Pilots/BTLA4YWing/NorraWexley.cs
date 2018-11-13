using Abilities.SecondEdition;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class NorraWexley : BTLA4YWing
        {
            public NorraWexley() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Norra Wexley",
                    5,
                    43,
                    limited: 1,
                    abilityType: typeof(NorraWexleyAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 13;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class NorraWexleyAbility : GenericAbility
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
            NorraWexleyAction newAction = new NorraWexleyAction()
            {
                Host = this.HostShip,
                Name = this.HostShip.PilotName + "'s Ability",
                DiceModificationName = this.HostShip.PilotName + "'s Ability"
            };
            ship.AddAvailableDiceModification(newAction);
        }

        private class NorraWexleyAction : ActionsList.GenericAction
        {
            public NorraWexleyAction()
            {
                //Name = DiceModificationName = "Norra Wexley's ability"; // Will be overwritten
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
                List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(Host, anotherShip);
                if (letters.Count > 0)
                {
                    return letters.First();
                }
                else
                {
                    return ' ';
                }
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                // Second edition Shara Bey only affects Primary Weapon Attacks
                if (Host.Owner.PlayerNo == Combat.Attacker.Owner.PlayerNo &&
                    RuleSets.RuleSet.Instance is RuleSets.SecondEdition &&
                    Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass))
                {
                    return false;
                }

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
    public class NorraWexleyAbility : Abilities.FirstEdition.NorraWexleyAbility
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
