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
                    41,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NorraWexleyAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 13
                );

                PilotNameCanonical = "norrawexley-btla4ywing";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NorraWexleyAbility : GenericAbility
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
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " gains 1 Evade die because at least one enemy is at range 1 of her");
                roll.AddDiceAndShow(DieSide.Success);
            }
        }
    }
}
