using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Tokens;
using BoardTools;

namespace Ship
{
    namespace ARC170
    {
        public class SharaBey : ARC170
        {
            public SharaBey() : base()
            {
                PilotName = "Shara Bey";
                PilotSkill = 6;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SharaBeyPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class SharaBeyPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGameStart += AddSharaBeyPilotAbility;

        }

        public void AddSharaBeyPilotAbility()
        {
            foreach (KeyValuePair<string, GenericShip> entry in HostShip.Owner.Ships)
            {
                if (entry.Value != HostShip)
                {
                    entry.Value.OnGenerateAvailableAttackPaymentList += AddSharaBeyPilotPayment;
                    entry.Value.OnGenerateDiceModifications += AddSharaBeyActionEffect;
                }
            }
        }

        public override void DeactivateAbility()
        {
            foreach (KeyValuePair<string, GenericShip> entry in HostShip.Owner.Ships)
            {
                if (entry.Value != HostShip)
                {
                    entry.Value.OnGenerateAvailableAttackPaymentList -= AddSharaBeyPilotPayment;
                }
            }
        }

        private void AddSharaBeyPilotPayment(List<GenericToken> waysToPay)
        {
            //char targetLockLetter = Actions.GetTargetLocksLetterPair(HostShip, Combat.Defender);
            if (!HostShip.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return;
            ShotInfo shotInfo = new ShotInfo(Selection.ActiveShip, HostShip, HostShip.PrimaryWeapon);
            if (shotInfo.Range <= 2)
            {
                GenericToken sharaToken = HostShip.Tokens.GetToken(typeof(BlueTargetLockToken), '*');
                waysToPay.Add(sharaToken);
            }
        }

        private void AddSharaBeyActionEffect(Ship.GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.SharaBeyAction()
            {
                Host = HostShip
            });
        }
    }
}

namespace ActionsList
{
    public class SharaBeyAction : GenericAction
    {
        private char sharaLockLetter;

        public SharaBeyAction()
        {
            Name = DiceModificationName = "Shara Bey";
        }

        public override bool IsDiceModificationAvailable()
        {
            ShotInfo shotInfo = new ShotInfo(Host, Combat.Attacker, Host.PrimaryWeapon);
            if (shotInfo.Range > 2) return false;
            sharaLockLetter = Actions.GetTargetLocksLetterPair(Host, Combat.Defender);
            if (sharaLockLetter == ' ') return false;
            GenericToken sharaToken = Host.Tokens.GetToken(typeof(BlueTargetLockToken), sharaLockLetter);
            if (sharaToken == null) return false;
            return Combat.AttackStep == CombatStep.Attack;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.DiceRollAttack.Blanks > 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetAvailableDiceModifications().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 30;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            
            Host.Tokens.SpendToken(typeof(BlueTargetLockToken), 
                                   delegate {
                                        DiceRerollManager diceRerollManager = new DiceRerollManager
                                        {
                                            CallBack = callBack
                                        };
                                        diceRerollManager.Start();
                                   }, sharaLockLetter);

        }

    }
}
