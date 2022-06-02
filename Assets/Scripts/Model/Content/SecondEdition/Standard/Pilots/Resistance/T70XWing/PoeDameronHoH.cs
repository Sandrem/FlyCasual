using ActionsList;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class PoeDameronHoH : T70XWing
        {
            public PoeDameronHoH() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Poe Dameron",
                    "Resistance Commander",
                    Faction.Resistance,
                    6,
                    6,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PoeDameronHoHAbility),
                    charges: 2,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    skinName: "Poe Dameron (RoS)"
                );

                PilotNameCanonical = "poedameron-swz68";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/76/66/76668017-6298-4471-b1cc-d388d5fc11b8/swz68_poe-dameron.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PoeDameronHoHAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (HostShip.State.Charges >= 2
                && HostShip.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo
                && Phases.CurrentPhase is MainPhases.ActivationPhase
                && (Phases.CurrentPhase as MainPhases.ActivationPhase).ActivationShip == Selection.ThisShip
            )
            {
                DistanceInfo distanceInfo = new DistanceInfo(HostShip, Selection.ThisShip);
                if (distanceInfo.Range <= 2)
                {
                    Selection.ThisShip.OnActionDecisionSubphaseEnd += DoAnotherAction;
                }
            }
        }

        private void DoAnotherAction(GenericShip ship)
        {
            Selection.ThisShip.OnActionDecisionSubphaseEnd -= DoAnotherAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            Selection.ThisShip.BeforeActionIsPerformed += PayChargeCost;

            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            List<GenericAction> whiteActionBarActionsAsRed = actions
                .Where(n => n.Color == Actions.ActionColor.White)
                .Select(n => n.AsRedAction)
                .ToList();

            Selection.ThisShip.AskPerformFreeAction(
                whiteActionBarActionsAsRed,
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After a friendly ship performs an action, Poe Dameron may spend 2 Charges to allow that ship to perform a white action, treating it as red",
                HostShip
            );
        }

        private void PayChargeCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.State.Charges -= 2;
            Selection.ThisShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            Selection.ThisShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }

    }
}