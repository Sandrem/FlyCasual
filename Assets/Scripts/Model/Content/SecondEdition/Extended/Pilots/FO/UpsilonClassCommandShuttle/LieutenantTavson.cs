using ActionsList;
using Arcs;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class LieutenantTavson : UpsilonClassCommandShuttle
        {
            public LieutenantTavson() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Tavson",
                    "Obedient Shuttle Pilot",
                    Faction.FirstOrder,
                    3,
                    8,
                    17,
                    isLimited: true,
                    charges: 2,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantTavsonPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
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

            HostShip.BeforeActionIsPerformed += SpendCharge;

            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActions(),
                delegate
                {
                    Selection.ThisShip = previousSelectedShip;
                    CleanUp();
                },
                HostShip.PilotInfo.PilotName,
                "After you suffer damage, you may spend 1 Charge to perform an action",
                HostShip
            );
        }

        private void SpendCharge(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= SpendCharge;
            HostShip.SpendCharge();
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= SpendCharge;
            Triggers.FinishTrigger();
        }
    }
}
