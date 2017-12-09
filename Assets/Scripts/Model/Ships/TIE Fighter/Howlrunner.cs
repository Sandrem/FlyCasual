using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace TIEFighter
    {
        public class Howlrunner : TIEFighter
        {
            public Howlrunner() : base()
            {
                PilotName = "\"Howlrunner\"";
                PilotSkill = 8;
                Cost = 18;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.HowlrunnerAbility());
            }
        }
    }
}

namespace Abilities
{
    public class HowlrunnerAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            GenericShip.AfterGenerateAvailableActionEffectsListGlobal += AddHowlrunnerAbility;
            HostShip.OnDestroyed += RemoveHowlrunnerAbility;
        }

        private void AddHowlrunnerAbility()
        {
            Combat.Attacker.AddAvailableActionEffect(new HowlrunnerAction() { Host = this.HostShip });
        }

        private void RemoveHowlrunnerAbility(GenericShip ship)
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal -= AddHowlrunnerAbility;
            HostShip.OnDestroyed -= RemoveHowlrunnerAbility;
        }

        private class HowlrunnerAction : ActionsList.GenericAction
        {
            public HowlrunnerAction()
            {
                Name = EffectName = "Howlrunner's ability";
                IsReroll = true;
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack)
                {
                    if (Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass))
                    {
                        if (Combat.Attacker.ShipId != Host.ShipId)
                        {
                            if (Combat.Attacker.Owner.PlayerNo == Host.Owner.PlayerNo)
                            {
                                Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(Host, Combat.Attacker);
                                if (positionInfo.Range == 1)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }

                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                    int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                    //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                    if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                    {
                        if (attackBlanks > 0) result = 90;
                    }
                    else
                    {
                        if (attackBlanks + attackFocuses > 0) result = 90;
                    }
                }

                return result;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = 1,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }
        }

    }
}
