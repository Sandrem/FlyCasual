using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Kickback : V19TorrentStarfighter
    {
        public Kickback()
        {
            PilotInfo = new PilotCardInfo(
                "\"Kickback\"",
                4,
                36,
                true,
                abilityType: typeof(Abilities.SecondEdition.KickbackAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/57/c4/57c43689-5d1f-4fd2-b1f6-d4bec9448634/swz32_kickback.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform a barrel roll action, you may perform a red lock action.
    public class KickbackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is BarrelRollAction)
            {
                HostShip.OnActionDecisionSubphaseEnd += PerformLockAction;
            }
        }

        private void PerformLockAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= PerformLockAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, AskPerformBoostAction);
        }

        private void AskPerformBoostAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostName + ": you may perform a red lock action");

            HostShip.AskPerformFreeAction(
                new TargetLockAction() { Color = ActionColor.Red },
                Triggers.FinishTrigger
            );
        }
    }
}
