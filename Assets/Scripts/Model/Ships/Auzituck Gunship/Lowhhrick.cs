﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class Lowhhrick : AuzituckGunship
        {
            public Lowhhrick() : base()
            {
                PilotName = "Lowhhrick";
                PilotSkill = 5;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Lowhhrick";

                PilotAbilities.Add(new Abilities.LowhhrickAbility());
            }
        }
    }
}

namespace Abilities
{
    public class LowhhrickAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal += AddLowhhrickAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal -= AddLowhhrickAbility;
        }

        private void AddLowhhrickAbility()
        {
            Combat.Defender.AddAvailableActionEffect(new DiceModificationAction() { Host = this.HostShip });
        }

        private class DiceModificationAction : ActionsList.GenericAction
        {
            public DiceModificationAction()
            {
                Name = EffectName = "Lowhhrick's ability";
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    if (Combat.Defender.ShipId != Host.ShipId)
                    {
                        if (Combat.Defender.Owner.PlayerNo == Host.Owner.PlayerNo)
                        {
                            if (Host.Tokens.HasToken(typeof(ReinforceForeToken)) || Host.Tokens.HasToken(typeof(ReinforceAftToken)))
                            {
                                Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(Host, Combat.Defender);
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

                /*if (Combat.AttackStep == CombatStep.Attack)
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
                }*/

                return result;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ApplyEvade();

                GenericToken reinforceToken = (Host.Tokens.HasToken(typeof(ReinforceForeToken))) ? Host.Tokens.GetToken(typeof(ReinforceForeToken)) : Host.Tokens.GetToken(typeof(ReinforceAftToken));
                Host.Tokens.SpendToken(reinforceToken.GetType(), callBack);
            }
        }

    }
}
