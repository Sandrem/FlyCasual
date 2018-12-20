using System;
using ActionsList;
using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Gonk : GenericUpgrade
    {
        public Gonk() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "GNK \"Gonk\" Droid",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GonkCrewAbility),
                charges: 1,
                seImageNumber: 43
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //Setup: Lose 1 charge.
    //Action: Recover 1 charge.
    //Action: Spend 1 charge to recover 1 shield.
    public class GonkCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSetupPlaced += OnSetupPlaced;
            HostShip.OnGenerateActions += AddActions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSetupPlaced -= OnSetupPlaced;
            HostShip.OnGenerateActions -= AddActions;
        }

        private void OnSetupPlaced(GenericShip ship)
        {
            HostUpgrade.State.SpendCharge();
        }

        private void AddActions(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 && HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax)
            {
                ship.AddAvailableAction(new RecoverShieldAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip,
                    Source = HostUpgrade,
                    Name = "Gonk: Spend 1 charge to recover 1 shield"
                });
            }
            
            if (HostUpgrade.State.Charges == 0)
            {
                ship.AddAvailableAction(new RecoverChargeAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip,
                    Source = HostUpgrade,
                    Name = "Gonk: Recover 1 charge"
                });
            }
        }

        private class RecoverChargeAction : GenericAction
        {            
            public override void ActionTake()
            {
                if (Source.State.Charges == 0)
                {
                    Source.State.RestoreCharge();
                }
                Phases.CurrentSubPhase.CallBack();
            }
        }

        private class RecoverShieldAction : GenericAction
        {
            public override void ActionTake()
            {
                if (Source.State.Charges > 0)
                {
                    Source.State.SpendCharge();
                    HostShip.TryRegenShields();
                }
                Phases.CurrentSubPhase.CallBack();
            }

        }
    }
}