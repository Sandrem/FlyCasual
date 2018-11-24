﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.ARC170
    {
        public class NorraWexley : ARC170
        {
            public NorraWexley() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Norra Wexley",
                    7,
                    29,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.NorraWexleyARC170Ability),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class NorraWexleyARC170Ability : GenericAbility
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
            NorraWexleyARC170Action newAction = new NorraWexleyARC170Action()
            {
                Host = this.HostShip,
                Name = this.HostShip.PilotName + "'s Ability",
                DiceModificationName = this.HostShip.PilotName + "'s Ability"
            };
            ship.AddAvailableDiceModification(newAction);
        }

        private class NorraWexleyARC170Action : GenericAction
        {
            public NorraWexleyARC170Action()
            {
                //Name = DiceModificationName = "Norra Wexley's ability"; // Will be overwritten
                TokensSpend.Add(typeof(BlueTargetLockToken));
            }

            public override void ActionEffect(System.Action callBack)
            {
                char targetLockPair = GetTargetLockTokenLetterOnAnotherShip();
                if (targetLockPair != ' ')
                {
                    Combat.CurrentDiceRoll.AddDice(DieSide.Focus).ShowWithoutRoll();
                    Combat.CurrentDiceRoll.OrganizeDicePositions();

                    Host.Tokens.SpendToken(typeof(BlueTargetLockToken), callBack, targetLockPair);
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
                    // TODOREVERT
                    //Host.GetType() == typeof(Ship.ARC170.SharaBey) &&
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