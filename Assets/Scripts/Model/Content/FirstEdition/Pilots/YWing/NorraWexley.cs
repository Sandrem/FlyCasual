using Mods.ModsList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YWing
    {
        public class NorraWexley : YWing
        {
            public NorraWexley() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "NorraWexley",
                    7,
                    24,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.NorraWexleyYWingAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                RequiredMods.Add(typeof(MyOtherRideIsMod));
                ImageUrl = "https://i.imgur.com/5HBK61g.png";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class NorraWexleyYWingAbility : GenericAbility
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
            NorraWexleyYWingAction newAction = new NorraWexleyYWingAction()
            {
                HostShip = this.HostShip,
                Name = this.HostShip.PilotInfo.PilotName + "'s Ability",
                DiceModificationName = this.HostShip.PilotInfo.PilotName + "'s Ability"
            };
            ship.AddAvailableDiceModification(newAction);
        }

        private class NorraWexleyYWingAction : ActionsList.GenericAction
        {
            public NorraWexleyYWingAction()
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

                    HostShip.Tokens.SpendToken(typeof(Tokens.BlueTargetLockToken), callBack, targetLockPair);
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
                List<char> letters = ActionsHolder.GetTargetLocksLetterPairs(HostShip, anotherShip);
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
                if (HostShip.Owner.PlayerNo == Combat.Attacker.Owner.PlayerNo &&
                    Editions.Edition.Current is Editions.SecondEdition &&
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
                    if (HostShip.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                    {
                        switch (Combat.AttackStep)
                        {
                            case CombatStep.Attack:
                                if (HostShip.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
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