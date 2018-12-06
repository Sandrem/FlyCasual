using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class NorraWexley : ARC170Starfighter
        {
            public NorraWexley() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Norra Wexley",
                    5,
                    55,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BraylenStrammAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 65
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NorraWexleyARC170Ability : Abilities.FirstEdition.NorraWexleyARC170Ability
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