using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class LieutenantSai : LambdaClassT4AShuttle
        {
            public LieutenantSai() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Sai",
                    "Death Squadron Veteran",
                    Faction.Imperial,
                    3,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantSaiAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 144,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
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
                Name = HostShip.PilotInfo.PilotName + "'s ability",
                TriggerType = TriggerTypes.OnActionIsPerformed,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = AbilityTakeFreeAction
            });
        }

        private void AbilityTakeFreeAction(object sender, EventArgs e)
        {
            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);

            HostShip.AskPerformFreeAction(
                abilityAction,
                delegate
                {
                    Selection.ChangeActiveShip(previousActiveShip);
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After you perform a Coordinate action, if the ship you chose performed an action on your action bar, you may perform that action",
                HostShip
            );
        }
    }
}
