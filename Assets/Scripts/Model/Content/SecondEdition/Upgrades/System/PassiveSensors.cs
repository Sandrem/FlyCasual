using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class PassiveSensors : GenericUpgrade
    {
        public PassiveSensors() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Passive Sensors",
                UpgradeType.System,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.PassiveSensorsAbility),
                regensCharges: true,
                charges: 1                
                //seImageNumber: 25
            );
            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            ImageUrl = "http://azrapse.es/Passive_Sensors.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // Action: Spend one charge. You can do this action only during the Perform Action Step. 
    // You cannot be coordinated while your charge is inactive
    // Before you engage, if your charge is inactive, you may perform a Calculate or Lock action.     
    public class PassiveSensorsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddPassiveSensorsAction;
            HostShip.OnCanBeCoordinated += CanBeCoordinatedCheck;
            HostShip.OnCombatActivation += BeforeEngageActionGrant;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddPassiveSensorsAction;
            HostShip.OnCanBeCoordinated -= CanBeCoordinatedCheck;
            HostShip.OnCombatActivation -= BeforeEngageActionGrant;
        }

        protected void AddPassiveSensorsAction(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                ship.AddAvailableAction(new PassiveSensorsAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip,
                    Source = HostUpgrade,
                    Name = "Spend Passive Sensors charge"
                });
            }                        
        }

        protected class PassiveSensorsAction : GenericAction
        {
            public override bool CanBePerformedAsAFreeAction
            {
                get
                {
                    return false;
                }
            }

            public override void ActionTake()
            {
                if (Source.State.Charges > 0)
                {
                    Source.State.SpendCharge();
                }
                Phases.CurrentSubPhase.CallBack();
            }
        }

        protected void CanBeCoordinatedCheck(GenericShip ship, ref bool canBeCoordinated)
        {
            if (HostUpgrade.State.Charges == 0)
            {
                canBeCoordinated = false;
            }
        }

        protected void BeforeEngageActionGrant(GenericShip ship)
        {
            if (HostUpgrade.State.Charges == 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, PerformPassiveSensorsGrantedAction);
            }
        }

        protected void PerformPassiveSensorsGrantedAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": You may perform Calculate or Lock action");
            List<GenericAction> grantedActions = new List<GenericAction>() { new CalculateAction(), new TargetLockAction() };

            HostShip.AskPerformFreeAction(grantedActions, Triggers.FinishTrigger);
        }

    }
}