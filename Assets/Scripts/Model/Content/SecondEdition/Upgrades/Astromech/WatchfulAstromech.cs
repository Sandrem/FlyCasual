using Actions;
using ActionsList;
using BoardTools;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class WatchfulAstromech : GenericUpgrade
    {
        public WatchfulAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Watchful Astromech",
                UpgradeType.Astromech,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.WatchfulAstromechAbility)
            );

            ImageUrl = "https://i.imgur.com/2txzWm1.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WatchfulAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if ((action is ReloadAction || action is RotateArcAction)
                && InEnemyFiringArc())
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskPerfromRedCalculate);
            }
        }

        private bool InEnemyFiringArc()
        {
            foreach (var anotherShip in HostShip.Owner.EnemyShips.Values)
            {
                ShotInfo shotInfo = new ShotInfo(anotherShip, HostShip, anotherShip.PrimaryWeapons);
                if (shotInfo.InArc) return true;
            }

            return false;
        }

        private void AskPerfromRedCalculate(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction
            (
                new CalculateAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger,
                descriptionShort: HostUpgrade.UpgradeInfo.Name,
                descriptionLong: "You may perform a red Calculate action",
                imageHolder: HostUpgrade
            );
        }
    }
}