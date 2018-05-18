﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using System;
using BoardTools;

namespace Ship
{
    namespace YT1300
    {
        public class Rey : YT1300
        {
            public Rey() : base()
            {
                PilotName = "Rey";
                PilotSkill = 8;
                Cost = 45;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                SubFaction = SubFaction.Resistance;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ReyAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ReyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += ReyPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= ReyPilotAbility;
        }

        public void ReyPilotAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ReyAction());
        }

        private class ReyAction : ActionsList.GenericAction
        {
            public ReyAction()
            {
                Name = EffectName = "Rey's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = new List<DieSide>() { DieSide.Blank },
                    NumberOfDiceCanBeRerolled = 2,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;

                switch (Combat.AttackStep)
                {
                    case CombatStep.Attack:
                        if (Combat.ShotInfo.InArc) result = true;
                        break;
                    case CombatStep.Defence:
                        ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                        if (shotInfo.InArc) result = true;
                        break;
                    default:
                        break;
                }

                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.CurrentDiceRoll.BlanksNotRerolled > 0) result = 95;

                return result;
            }

        }
    }
}
