using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace YWing
    {
        public class HortonSalm : YWing
        {
            public HortonSalm() : base()
            {
                PilotName = "Horton Salm";
                PilotSkill = 8;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                faction = Faction.Rebel;

                SkinName = "Gray";

                PilotAbilities.Add(new Abilities.HortonSalmAbility());
            }
        }
    }
}

namespace Abilities
{
    public class HortonSalmAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.AfterGenerateAvailableActionEffectsList += HortonSalmPilotAbility;
        }

        public void HortonSalmPilotAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new HortonSalmAction());
        }

        private class HortonSalmAction : ActionsList.GenericAction
        {
            public HortonSalmAction()
            {
                Name = EffectName = "Horton Salm's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = new List<DieSide> { DieSide.Blank },
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if ((Combat.AttackStep == CombatStep.Attack) && (shotInfo.Range > 1))
                {
                    result = true;
                }
                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    if (Combat.DiceRollAttack.Blanks > 0) result = 95;
                }

                return result;
            }
        }

    }
}
