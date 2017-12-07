using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace TIEFighter
    {
        public class WingedGundark : TIEFighter
        {
            public WingedGundark() : base()
            {
                PilotName = "\"Winged Gundark\"";
                PilotSkill = 5;
                Cost = 15;

                IsUnique = true;

                PilotAbilities.Add(new AbilitiesNamespace.WingedGundarkAbility());
            }
        }
    }
}

namespace AbilitiesNamespace
{
    public class WingedGundarkAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.AfterGenerateAvailableActionEffectsList += WingedGundarkPilotAbility;
        }

        private void WingedGundarkPilotAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new WingedGundarkAction());
        }

        private class WingedGundarkAction : ActionsList.GenericAction
        {

            public WingedGundarkAction()
            {
                Name = EffectName = "\"Winged Gundark\"'s ability";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
                callBack();
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack)
                {
                    Board.ShipShotDistanceInformation shotInformation = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                    if (shotInformation.Range == 1)
                    {
                        result = true;
                    }
                }
                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 20;
                }

                return result;
            }

        }
    }
}
