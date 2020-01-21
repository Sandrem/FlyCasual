using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PrecognitiveReflexes : GenericUpgrade, IVariableCost
    {
        public PrecognitiveReflexes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Precognitive Reflexes",
                UpgradeType.ForcePower,
                cost: 13,
                abilityType: typeof(Abilities.SecondEdition.PrecognitiveReflexesAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/de/c2/dec27d21-73b1-4b44-b61e-78188a5555cb/swz48_cards-precog-reflexes.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 3},
                {1, 3},
                {2, 3},
                {3, 4},
                {4, 7},
                {5, 10},
                {6, 13}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PrecognitiveReflexesAbility : FirstEdition.SabineWrenPilotAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskPerformFreeAction);
        }

        private void AskPerformFreeAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.BeforeActionIsPerformed += PayForceToken;
                HostShip.OnActionIsPerformed += CheckDebuff;
                HostShip.OnActionIsSkipped += DeregisterDebuffEvents;

                HostShip.AskPerformFreeAction
                (
                    new List<GenericAction>()
                    {
                        new BoostAction(),
                        new BarrelRollAction()
                    },
                    Triggers.FinishTrigger,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may spend 1 Force to perform a Barrel Roll or Boost action. Then, if you performed an action you do not have on your action bar, gain 1 strain token. Skip your action during Activation.",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PayForceToken(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= PayForceToken;

            HostShip.State.Force--;
        }

        private void DeregisterDebuffEvents(GenericShip ship)
        {
            HostShip.BeforeActionIsPerformed -= PayForceToken;
            HostShip.OnActionIsPerformed -= CheckDebuff;
            HostShip.OnActionIsSkipped -= DeregisterDebuffEvents;
        }

        public void CheckDebuff(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= CheckDebuff;

            if (!HostShip.ActionBar.HasAction(action.GetType()))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, GainStrainToken);
            }

            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You cannot perform another actions during your activation");

            HostShip.OnTryAddAction += DisallowAction;
            HostShip.OnMovementActivationFinish += ClearRestriction;
        }

        private void DisallowAction(GenericShip ship, GenericAction action, ref bool isAllowed)
        {
            isAllowed = false;
        }

        private void ClearRestriction(GenericShip ship)
        {
            Messages.ShowInfoToHuman(HostUpgrade.UpgradeInfo.Name + ": You can perform actions as usual");

            HostShip.OnMovementActivationFinish -= ClearRestriction;
            HostShip.OnTryAddAction -= DisallowAction;
        }
        private void GainStrainToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": This ship gains 1 Strain token for performing an action not on its action bar");

            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }
    }
}