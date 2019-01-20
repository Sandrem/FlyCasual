using ActionsList;
using Arcs;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class LieutenantTavson : UpsilonClassCommandShuttle
        {
            public LieutenantTavson() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Tavson",
                    3,
                    62,
                    isLimited: true,
                    charges: 2,
                    regensCharges: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantTavsonPilotAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/20fbf3ed79c50d2082cdb44caac26064.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you suffer damage, you may spend 1 charge to perform an action.
    public class LieutenantTavsonPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageWasSuccessfullyDealt += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageWasSuccessfullyDealt -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, bool flag)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageWasSuccessfullyDealt, PerformAction);
            }
        }
        private void PerformAction(object sender, System.EventArgs e)
        {
            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            Messages.ShowInfoToHuman(HostName + ": you may spend 1 charge to perform an action");

            HostShip.BeforeFreeActionIsPerformed += SpendCharge;

            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), delegate
            {
                Selection.ThisShip = previousSelectedShip;
                CleanUp();
            });
        }

        private void SpendCharge(GenericAction action)
        {
            HostShip.BeforeFreeActionIsPerformed -= SpendCharge;
            HostShip.SpendCharge();
        }

        private void CleanUp()
        {
            HostShip.BeforeFreeActionIsPerformed -= SpendCharge;
            Triggers.FinishTrigger();
        }
    }
}
