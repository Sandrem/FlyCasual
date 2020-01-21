using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Virago : GenericUpgrade
    {
        public Virago() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Virago",
                UpgradeType.Title,
                cost: 8,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.StarViper.StarViper)),
                addSlot: new UpgradeSlot(UpgradeType.Modification),
                addShields: 1,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.ViragoAbility),
                seImageNumber: 155
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ViragoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.State.Charges > 0 && !HostShip.IsStressed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;

            Selection.ChangeActiveShip(HostShip);

            HostShip.AskPerformFreeAction(
                new BoostAction() { Color = ActionColor.Red, CanBePerformedWhileStressed = false },
                CleanUp,
                HostUpgrade.UpgradeInfo.Name,
                "During the End Phase, you may spend 1 Charge to perform a red Boost action",
                HostUpgrade
            );
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            if (action is BoostAction && isFreeAction)
            {
                HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;

                RegisterAbilityTrigger(
                    TriggerTypes.BeforeActionIsPerformed,
                    delegate {
                        HostUpgrade.State.SpendCharge();
                        Triggers.FinishTrigger();
                    }
                );
            }
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;

            Triggers.FinishTrigger();
        }
    }
}