using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class Sigma4BoY : TIEInterceptor
        {
            public Sigma4BoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sigma 4",
                    "Battle of Yavin",
                    Faction.Imperial,
                    4,
                    4,
                    0,
                    charges: 2,
                    isLimited: true,
                    abilityType: typeof(Sigma4BoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                AutoThrustersAbility oldAbility = (AutoThrustersAbility) ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                //oldAbility.DeactivateAbility();
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsBoYRealAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Disciplined));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.PrimedThrusters));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/ae/Sigma4-battleofyavin.png";

                PilotNameCanonical = "sigma4-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Sigma4BoYAbility : GenericAbility
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
            if (HostShip.State.Charges > 0
                && action is BarrelRollAction)
            {
                HostShip.OnActionDecisionSubphaseEnd += ProposeBoostAction;
            }
        }

        private void ProposeBoostAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= ProposeBoostAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += PayChargeCost;

            HostShip.AskPerformFreeAction
            (
                new BoostAction() { HostShip = HostShip },
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you perform a Barrel Roll action, you may spend 1 charge to perform a Boost action"
            );
        }

        private void PayChargeCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.SpendCharge();
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }
    }
}