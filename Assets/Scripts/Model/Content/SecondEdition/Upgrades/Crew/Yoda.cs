using Ship;
using Upgrade;
using ActionsList;
using Actions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class Yoda : GenericUpgrade
    {
        public Yoda() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Yoda",
                UpgradeType.Crew,
                cost: 12,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.YodaCrewAbility),
                addForce: 2
            );

            NameCanonical = "yoda-republic";

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(241, 0),
                new Vector2(125, 125)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/27/8a/278a7c83-c1e0-4ea4-b36b-2114e95fde99/swz70_a1_yoda_upgrade.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class YodaCrewAbility : GenericAbility
    {
        //After another friendly ship at range 0-2 fully executes a purple maneuver or performs a purple action, you may spend 1 Force. 
        //If you do, that ship recovers 1 Force.
        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal += CheckAbilityManeuver;
            GenericShip.OnActionIsPerformedGlobal += CheckAbilityAction;
            
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal -= CheckAbilityManeuver;
            GenericShip.OnActionIsPerformedGlobal -= CheckAbilityAction;
        }


        private void CheckAbilityManeuver(GenericShip ship)
        {
            if (HostShip.State.Force > 0
                && ship != HostShip
                && ship.GetLastManeuverColor() == Movement.MovementComplexity.Purple 
                && ship.Owner == HostShip.Owner
                && ship.GetRangeToShip(HostShip) <= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, delegate { AskToRecoverForce(ship); });
            }
        }


        private void CheckAbilityAction(GenericAction action)
        {
            if (HostShip.State.Force > 0
                && action.HostShip != HostShip
                && action.Color == ActionColor.Purple 
                && action.HostShip.Owner == HostShip.Owner
                && action.HostShip.GetRangeToShip(HostShip) <= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, delegate { AskToRecoverForce(action.HostShip); });
            }
        }

        private void AskToRecoverForce(GenericShip targetShip)
        {
            if (HostShip.State.Force > 0)
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault, //TODO: better AI decision?
                    delegate { RecoverForce(targetShip); },
                    descriptionLong: "Spend 1 Force from " + HostUpgrade.HostShip.PilotInfo.PilotName + " to recover 1 Force for " + targetShip.PilotInfo.PilotName,
                    imageHolder: HostUpgrade
                );
            else
                Triggers.FinishTrigger();
        }

        private void RecoverForce(GenericShip targetShip)
        {
            if (HostShip.State.Force > 0 && targetShip.State.Force < targetShip.State.MaxForce)
            {
                targetShip.State.RestoreForce();
                HostShip.State.SpendForce(1, SubPhases.DecisionSubPhase.ConfirmDecision);
            }
            else
            {
                SubPhases.DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}