using Ship;
using Upgrade;
using System.Linq;
using System;
using System.Collections.Generic;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class MoffJerjerrod : GenericUpgrade
    {
        public MoffJerjerrod() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Moff Jerjerrod",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Imperial),
                    new ActionBarRestriction(typeof(ActionsList.CoordinateAction))
                ),
                abilityType: typeof(Abilities.SecondEdition.MoffJerjerrodAbility),
                charges: 2,
                regensCharges: true,
                seImageNumber: 120
            );

            // TODO: Avatar
            // Avatar = new AvatarInfo(Faction.Imperial, new Vector2(385, 11));
        }
    }
}
namespace Abilities.SecondEdition
{

    public class MoffJerjerrodAbility : GenericAbility
    {
        private string SelectedTemplateName;

        public override void ActivateAbility()
        {
            Phases.Events.OnSystemsPhaseStart += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSystemsPhaseStart -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.State.Charges >= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                NeverUseByDefault,
                UseAbility,
                infoText: "You may spend 2 charges. If you do, choose a template. Each friendly ship may perform a red boost action using that template."
            );
        }

        private void UseAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharges(2);
            AskSelectTemplate();
        }

        private void AskSelectTemplate()
        {
            BoostTemplateDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<BoostTemplateDecisionSubphase>(
                "Select a template",
                TemplateIsSelected
            );

            List<string> decisions = new List<string>() { "Straight 1", "Bank 1 Left", "Bank 1 Right" };
            foreach (string item in decisions)
            {
                subphase.AddDecision(item, delegate { SelectTemplate(item); }, isCentered: (item == "Straight 1"));
            }

            subphase.InfoText = "Choose a template - each friendly ship may perform a red boost action using that template";
            subphase.DefaultDecisionName = decisions.First();
            subphase.DecisionOwner = HostShip.Owner;

            subphase.Start();
        }

        private void SelectTemplate(string selectedTemplateName)
        {
            SelectedTemplateName = selectedTemplateName;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void TemplateIsSelected()
        {
            SelectTargetRecursive();
        }

        private void SelectTargetRecursive()
        {
            Selection.ChangeActiveShip(HostShip);

            if (HostShip.Owner.Ships.Any(s => FilterTargets(s.Value)))
            {
                SelectTargetForAbility(
                    TryToPerformBoost,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostUpgrade.UpgradeInfo.Name,
                    "Choose a friendly ship to perform a red boost action using selected template",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void TryToPerformBoost()
        {
            Selection.ChangeActiveShip(TargetShip);
            Selection.ThisShip.AskPerformFreeAction(
                new BoostAction() {
                    Color = Actions.ActionColor.Red,
                    SelectedBoostTemplate = SelectedTemplateName
                },
                delegate
                {
                    SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();
                    SelectTargetRecursive();
                },
                isForced: true
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return HostShip.Owner.PlayerNo == ship.Owner.PlayerNo && ship.CanPerformAction(new BoostAction() { Color = Actions.ActionColor.Red });
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0; // No AI
        }

        private class BoostTemplateDecisionSubphase : SubPhases.DecisionSubPhase { }
    }
}