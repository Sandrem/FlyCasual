using ActionsList;
using Ship;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class LieutenantSai : LambdaClassT4AShuttle
        {
            public LieutenantSai() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Sai",
                    3,
                    47,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantSaiAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 144
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantSaiAbility : GenericAbility
    {
        GenericShip abilityTarget;
        GenericAction abilityAction;
        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += RegisterAbilityEvents;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= RegisterAbilityEvents;
        }

        private void RegisterAbilityEvents(GenericShip targetShip)
        {
            abilityTarget = targetShip;
            targetShip.OnActionIsPerformed += RegisterAbility;
            targetShip.OnActionIsSkipped += DeregisterAbilityEvents;
        }

        private void DeregisterAbilityEvents(GenericShip ship)
        {
            abilityTarget.OnActionIsPerformed -= RegisterAbility;
            abilityTarget.OnActionIsSkipped -= DeregisterAbilityEvents;
            abilityTarget = null;
            abilityAction = null;
        }

        private void RegisterAbility(GenericAction action)
        {

            DeregisterAbilityEvents(abilityTarget);

            if (action == null || !HostShip.ActionBar.HasAction(action.GetType()))
            {
                return;
            }

            abilityAction = action;
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Lieutenant Sai's ability",
                TriggerType = TriggerTypes.OnActionIsPerformed,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = AbilityTakeFreeAction
            });
        }

        private void AbilityTakeFreeAction(object sender, EventArgs e)
        {
            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);
            HostShip.AskPerformFreeAction(abilityAction, delegate
            {
                Selection.ChangeActiveShip(previousActiveShip);
                Triggers.FinishTrigger();
            });
        }
    }
}
