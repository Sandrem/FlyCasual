using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obstacles;
using Players;
using Ship;
using Tokens;
using UnityEngine;

namespace Obstacles
{
    public class GasCloud : GenericObstacle
    {
        public GasCloud(string name, string shortName) : base(name, shortName)
        {
            
        }

        public override string GetTypeName => "Gas Cloud";

        public override void OnHit(GenericShip ship)
        {
            Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " hit a gas cloud during movement, their action subphase is skipped");
            Selection.ThisShip.IsSkipsActionSubPhase = true;

            Triggers.FinishTrigger();
        }

        public override void OnLanded(GenericShip ship)
        {
            // Nothing
        }

        public override void OnShotObstructedExtra(GenericShip attacker, GenericShip defender)
        {
            defender.OnGenerateDiceModifications += TryToAddDiceModification;
        }

        private void TryToAddDiceModification(GenericShip ship)
        {
            ActionsList.GasCloudDiceModification newAction = new ActionsList.GasCloudDiceModification()
            {
                HostShip = ship,
                HostObstacle = this
            };
            ship.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class GasCloudDiceModification : GenericAction
    {
        public GenericObstacle HostObstacle;

        public GasCloudDiceModification()
        {
            Name = DiceModificationName = "Gas Cloud";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence
                && Combat.CurrentDiceRoll.Blanks > 0
                && Combat.ShotInfo.ObstaclesObstructed.Contains(HostObstacle)
            )
            {
                result = true;
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.CurrentDiceRoll.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    return 100;
                }
            }

            return result;
        }

    }
}
