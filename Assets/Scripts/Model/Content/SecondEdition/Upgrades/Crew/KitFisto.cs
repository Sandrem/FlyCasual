using Ship;
using Upgrade;
using ActionsList;
using Actions;
using System;
using SubPhases;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class KitFisto : GenericUpgrade
    {
        public KitFisto() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kit Fisto",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(EvadeAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.KitFistoAbility),
                addForce: 1
            );

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(261, 1)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/18/f5/18f5a7f6-8fce-4dba-b6cf-f5c739f807ca/swz70_a1_kit-fisto_upgrade.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KitFistoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectShip);
            }
        }

        private void AskToSelectShip(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                AskToPerformRedEvadeAction,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: "Kit Fisto",
                description: "You may spend 1 force and choose a ship - it may perform red Evade action",
                imageSource: HostUpgrade
            );
        }

        private void AskToPerformRedEvadeAction()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            
            Selection.ChangeActiveShip(TargetShip);

            HostShip.State.SpendForce(
                1,
                delegate
                {
                    TargetShip.AskPerformFreeAction(
                        new EvadeAction() { Color = ActionColor.Red },
                        Triggers.FinishTrigger,
                        descriptionShort: "Kit Fisto",
                        descriptionLong: "You must perform red Evade action",
                        imageHolder: HostUpgrade,
                        isForced: true
                    );
                }
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly, TargetTypes.This)
                && FilterTargetsByRange(ship, minRange: 0, maxRange: 1);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        
    }
}