using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using System.Linq;
using Ship;
using Abilities;

namespace UpgradesList
{

    public class FlightAssistAstromech : GenericUpgrade
    {
        public FlightAssistAstromech() : base()
        {
            Type = UpgradeType.Astromech;
            Name = "Flight-Assist Astromech";
            Cost = 1;

            UpgradeAbilities.Add(new FlightAssistAstromechAbility());
        }
    }

}

namespace Abilities
{
    public class FlightAssistAstromechAbility: GenericAbility
    {
        private List<IShipWeapon> turnedOffOutOfArcWeapons = new List<IShipWeapon>();

        public override void ActivateAbility()
        {
            SetCannotAttackOutsideArc();
            HostShip.OnMovementFinish += RegisterFlightAssistAstromechAbility;
        }

        public override void DeactivateAbility()
        {
            foreach (var weapon in turnedOffOutOfArcWeapons)
            {
                weapon.CanShootOutsideArc = true;
            }

            HostShip.OnMovementFinish -= RegisterFlightAssistAstromechAbility;
        }

        private void SetCannotAttackOutsideArc()
        {
            foreach (IShipWeapon weapon in HostShip.UpgradeBar.GetInstalledUpgrades().Where(n => n is IShipWeapon))
            {
                if (weapon.CanShootOutsideArc)
                {
                    turnedOffOutOfArcWeapons.Add(weapon);
                    weapon.CanShootOutsideArc = false;
                }
            }
        }

        private void RegisterFlightAssistAstromechAbility(GenericShip host)
        {
            if (!Selection.ThisShip.IsBumped && !Selection.ThisShip.IsHitObstacles && IsNoEnemyInArcAndDistance())
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementExecuted, AskPerformFreeActions);
            }
        }

        private bool IsNoEnemyInArcAndDistance()
        {
            bool result = true;

            foreach (var enemyShip in HostShip.Owner.EnemyShips)
            {
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(HostShip, enemyShip.Value);
                if (shotInfo.InArc && shotInfo.Range >= 1 && shotInfo.Range <= 3)
                {
                    return false;
                }
            }

            return result;
        }

        private void AskPerformFreeActions(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.BoostAction(), new ActionsList.BarrelRollAction() };

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

    }
}
