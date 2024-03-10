using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class JakeFarrellSSP : RZ1AWing
        {
            public JakeFarrellSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jake Farrell",
                    "Sage Instructor",
                    Faction.Rebel,
                    4,
                    5,
                    0,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JakeFarrellSSPAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    skinName: "Blue",
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(Elusive));
                MustHaveUpgrades.Add(typeof(Outmaneuver));
                MustHaveUpgrades.Add(typeof(IonMissiles));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/jakefarrell-swz106.png";

                PilotNameCanonical = "jakefarrell-swz106";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JakeFarrellSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is BoostAction || action is BarrelRollAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SelectTargetForJakeFarrellSSPAbility);
            }
        }

        private void SelectTargetForJakeFarrellSSPAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                GrantFreeFocusAction,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a friendly ship, it may perform a free Focus action",
                HostShip
            );
        }

        private void GrantFreeFocusAction()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            Selection.ThisShip = TargetShip;
            TargetShip.AskPerformFreeAction(
                new FocusAction() { HostShip = TargetShip },
                AfterFreeFocusAction,
                HostShip.PilotInfo.PilotName,
                "You may perform a Focus action",
                HostShip
            );
        }

        private void AfterFreeFocusAction()
        {
            Selection.ThisShip = HostShip;
            Triggers.FinishTrigger();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.This, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;

            if (ship.IsStressed) return priority;

            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) priority += 100;

            priority += ship.PilotInfo.Cost;

            return priority;
        }
    }
}