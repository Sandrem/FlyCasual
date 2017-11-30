using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using System.Linq;
using Ship;

namespace UpgradesList
{

    public class FlightAssistAstromech : GenericUpgrade
    {
        private List<IShipWeapon> turnedOffOutOfArcWeapons = new List<IShipWeapon>();

        public FlightAssistAstromech() : base()
        {
            Type = UpgradeType.Astromech;
            Name = "Flight-Assist Astromech";
            Cost = 1;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Phases.OnGameStart += ActivateAbility;
        }

        private void ActivateAbility()
        {
            SetCannotAttackOutsideArc();
            Host.OnMovementFinish += FlightAssistAstromechAbility;
        }

        private void DeactivateAbility()
        {
            foreach (var weapon in turnedOffOutOfArcWeapons)
            {
                weapon.CanShootOutsideArc = true;
            }

            Host.OnMovementFinish -= FlightAssistAstromechAbility;
        }

        private void SetCannotAttackOutsideArc()
        {
            foreach (IShipWeapon weapon in Host.UpgradeBar.GetInstalledUpgrades().Where(n => n is IShipWeapon))
            {
                if (weapon.CanShootOutsideArc)
                {
                    turnedOffOutOfArcWeapons.Add(weapon);
                    weapon.CanShootOutsideArc = false;
                }
            }
        }

        private void FlightAssistAstromechAbility(GenericShip host)
        {
            if (!Selection.ThisShip.IsBumped && !Selection.ThisShip.IsHitObstacles && IsNoEnemyInArcAndDistance())
            {
                Triggers.RegisterTrigger(new Trigger() {
                    Name = "R2-D2: Regen Shield",
                    TriggerOwner = host.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnShipMovementExecuted,
                    EventHandler = AskPerformFreeActions
                });
            }
        }

        private bool IsNoEnemyInArcAndDistance()
        {
            bool result = true;

            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(Host.Owner.PlayerNo)).Ships)
            {
                Debug.Log("Checking " + enemyShip.Value.ShipId);
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, enemyShip.Value, Host.PrimaryWeapon);
                if (shotInfo.InArc && shotInfo.Range >= 1 && shotInfo.Range <= 3)
                {
                    Debug.Log("!!! SEE " + enemyShip.Value.ShipId);
                    return false;
                }
            }

            return result;
        }

        private void AskPerformFreeActions(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.BoostAction(), new ActionsList.BarrelRollAction() };

            Host.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

        public override void Discard(Action callBack)
        {
            DeactivateAbility();

            base.Discard(callBack);
        }

    }

}
