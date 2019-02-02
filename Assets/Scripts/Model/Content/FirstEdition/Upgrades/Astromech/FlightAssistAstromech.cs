using Upgrade;
using Ship;
using System.Collections.Generic;
using System.Linq;
using BoardTools;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class FlightAssistAstromech : GenericUpgrade
    {
        public FlightAssistAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Flight-Assist Astromech",
                UpgradeType.Astromech,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.FlightAssistAstromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class FlightAssistAstromechAbility : GenericAbility
    {
        private List<IShipWeapon> turnedOffOutOfArcWeapons = new List<IShipWeapon>();

        public override void ActivateAbility()
        {
            SetCannotAttackOutsideArc();
            HostShip.OnMovementFinish += RegisterFlightAssistAstromechAbility;
        }

        public override void DeactivateAbility()
        {
            // TODOREVERT

            /*foreach (var weapon in turnedOffOutOfArcWeapons)
            {
                weapon.CanShootOutsideArc = true;
            }*/

            HostShip.OnMovementFinish -= RegisterFlightAssistAstromechAbility;
        }

        private void SetCannotAttackOutsideArc()
        {
            /*foreach (IShipWeapon weapon in HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Where(n => n is IShipWeapon))
            {
                if (weapon.CanShootOutsideArc)
                {
                    turnedOffOutOfArcWeapons.Add(weapon);
                    weapon.CanShootOutsideArc = false;
                }
            }*/
        }

        private void RegisterFlightAssistAstromechAbility(GenericShip host)
        {
            if (Selection.ThisShip.IsBumped) return;
            if (Selection.ThisShip.IsHitObstacles) return;
            if (!IsNoEnemyInArcAndDistance()) return;
            if (BoardTools.Board.IsOffTheBoard(host)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementExecuted, AskPerformFreeActions);
        }

        private bool IsNoEnemyInArcAndDistance()
        {
            bool result = true;

            foreach (var enemyShip in HostShip.Owner.EnemyShips)
            {
                ShotInfo shotInfo = new ShotInfo(HostShip, enemyShip.Value, HostShip.PrimaryWeapons);
                if (shotInfo.InArc && shotInfo.Range >= 1 && shotInfo.Range <= 3)
                {
                    return false;
                }
            }

            return result;
        }

        private void AskPerformFreeActions(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Astromech-Beeping-and-whistling");

            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new BoostAction(),
                    new BarrelRollAction()
                },
                Triggers.FinishTrigger);
        }

    }
}