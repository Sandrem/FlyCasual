using Upgrade;
using Ship;
using GameModes;
using System.Collections.Generic;

namespace UpgradesList
{

    public class BB8 : GenericUpgrade
    {

        public BB8() : base()
        {
            Type = UpgradeType.Astromech;
            Name = "BB-8";
            isUnique = true;
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnManeuverIsRevealed += AddAction;
        }

        private void AddAction(Ship.GenericShip host)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "BB-8's ability",
                TriggerType = TriggerTypes.OnManeuverIsRevealed,
                TriggerOwner = host.Owner.PlayerNo,
                EventHandler = AskPerformFreeActions
            });
        }

        private void AskPerformFreeActions(object sender, System.EventArgs e)
        {
            Host.AskPerformFreeAction(new List<ActionsList.GenericAction>() { new ActionsList.BarrelRollAction() }, Triggers.FinishTrigger);
        }
    }
}