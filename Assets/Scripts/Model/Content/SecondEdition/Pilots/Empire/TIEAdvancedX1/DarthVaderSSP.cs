using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class DarthVaderSSP : TIEAdvancedX1
        {
            public DarthVaderSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Darth Vader",
                    "Black Leader",
                    Faction.Imperial,
                    6,
                    6,
                    0,
                    isLimited: true,
                    abilityType: typeof(DarthVaderSSPAbility),
                    force: 3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie,
                        Tags.DarkSide,
                        Tags.Sith
                    },
                    skinName: "Blue",
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(Hate));
                MustHaveUpgrades.Add(typeof(IonMissiles));
                MustHaveUpgrades.Add(typeof(AfterBurners));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/darthvader-swz105.png";

                PilotNameCanonical = "darthvader-swz105";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may spend 1 force to perform an action.

    public class DarthVaderSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.OnActionDecisionSubphaseEnd += DoAnotherAction;
            }
        }

        private void DoAnotherAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoAnotherAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += PayForceCost;

            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(
                actions,
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you perform an action, you may spend 1 Force to perform an action",
                HostShip
            );
        }

        private void PayForceCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= PayForceCost;
            RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, SpendForce);
        }

        private void SpendForce(object sender, EventArgs e)
        {
            HostShip.State.SpendForce(1, Triggers.FinishTrigger);
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayForceCost;
            Triggers.FinishTrigger();
        }

    }
}
