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
                    abilityType: typeof(Abilities.SecondEdition.NorraWexleyYWingAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 13
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NorraWexleyYWingAbility : Abilities.FirstEdition.NorraWexleyYWingAbility
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
