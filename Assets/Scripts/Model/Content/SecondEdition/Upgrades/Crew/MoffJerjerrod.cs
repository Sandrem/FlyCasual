using Ship;
using Upgrade;
using System.Linq;
using System;
using System.Collections.Generic;
using ActionsList;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class MoffJerjerrod : GenericUpgrade
    {
        public MoffJerjerrod() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Moff Jerjerrod",
                UpgradeType.Crew,
                cost: 7,
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

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(417, 4),
                new Vector2(150, 150)
            );
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
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            if (HostUpgrade.State.Charges >= 2) flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges >= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseAbility,
                descriptionLong: "Do you may spend 2 charges? (If you do, choose a template. Each friendly ship may perform a red boost action using that template)",
                imageHolder: HostUpgrade
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

            subphase.DescriptionShort = "Moff Jerjerrod";
            subphase.DescriptionLong = "Choose a template - each friendly ship may perform a red boost action using that template";
            subphase.ImageSource = HostUpgrade;

            List<string> decisions = new List<string>() { "Straight 1", "Bank 1 Left", "Bank 1 Right" };
            foreach (string item in decisions)
            {
                subphase.AddDecision(item, delegate { SelectTemplate(item); }, isCentered: (item == "Straight 1"));
            }

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
                    "Choose a friendly ship to perform a red Boost action using selected template",
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
                HostUpgrade.UpgradeInfo.Name,
                "You must perform a red Boost action using selected template",
                HostUpgrade,
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