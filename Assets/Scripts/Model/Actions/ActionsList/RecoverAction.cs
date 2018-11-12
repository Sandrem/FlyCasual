using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Tokens;
using System.Linq;
using System;
using Ship;

namespace ActionsList
{

    public class RecoverAction : GenericAction
    {

        public RecoverAction()
        {
            Name = DiceModificationName = "Recover";
        }

        public override void ActionTake()
        {
            SpendEnergyForShields();
        }

        private void SpendEnergyForShields()
        {
            var ship = Selection.ThisShip;
            var energyTokenCount = ship.Energy;
            //var maxRecoverableShields = ship.MaxShields - ship.Shields;            

            SpendEnergy(energyTokenCount, Selection.ThisShip);
        }

        private void SpendEnergy(int count, GenericShip ship)
        {
            if (count > 0)
            {
                ship.Tokens.SpendToken(typeof(EnergyToken), () => RecoverShield(ship, () => SpendEnergy(count - 1, ship)));
            }
            else
            {
                Phases.CurrentSubPhase.CallBack();
            }
        }

        private void RecoverShield(GenericShip ship, Action callback)
        {
            ship.TryRegenShields();
            callback();
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int recoverableShields = Mathf.Min(Selection.ThisShip.State.ShieldsMax - Selection.ThisShip.State.ShieldsCurrent, Selection.ThisShip.Energy);
            result = recoverableShields * 110;

            return result;
        }

    }

}
