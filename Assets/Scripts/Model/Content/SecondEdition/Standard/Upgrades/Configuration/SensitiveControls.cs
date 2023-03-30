using Upgrade;
using ActionsList;
using Actions;
using System.Collections.Generic;
using Ship;
using System;
using Arcs;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class SensitiveControls : GenericUpgrade
    {
        public SensitiveControls() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sensitive Controls",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new AbilityPresenceRestriction(typeof(Abilities.SecondEdition.AutoThrustersAbility)),
                abilityType: typeof(Abilities.SecondEdition.SensitiveControlsHolderAbility),
                isStandardazed: true
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a7/e5/a7e50b41-b3d2-44d5-ad8d-c9b2028e5fc6/swz84_upgrade_sensitivecontrols.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SensitiveControlsHolderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AutoThrustersAbility oldAbility = (AutoThrustersAbility) HostShip.ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
            oldAbility.DeactivateAbility();
            HostShip.ShipAbilities.Remove(oldAbility);

            GenericAbility ability = new SensitiveControlsRealAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }

        public override void DeactivateAbility()
        {
            HostShip.ShipAbilities.RemoveAll(n => n.GetType() == typeof(SensitiveControlsRealAbility));

            GenericAbility ability = new AutoThrustersAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }
    }

    public class SensitiveControlsRealAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToPerformTwoRedActions);
        }

        private void AskToPerformTwoRedActions(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<GenericAction>() {
                    new BarrelRollAction(){ HostShip = HostShip, Color = ActionColor.Red },
                    new BoostAction(){ HostShip = HostShip, Color = ActionColor.Red }
                },
                Triggers.FinishTrigger,
                descriptionShort: "Sensitive Controls",
                descriptionLong: "You may perform a red barrel roll or boost action",
                imageHolder: HostUpgrade
            );
        }
    }

    public class SensitiveControlsBoYRealAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger
            (
                TriggerTypes.OnSystemsAbilityActivation,
                AskToPerformTwoRedActions,
                customTriggerName: "Sensitive Controls"
            );
        }

        private void AskToPerformTwoRedActions(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<GenericAction>() {
                    new BarrelRollAction(){ HostShip = HostShip, Color = ActionColor.Red },
                    new BoostAction(){ HostShip = HostShip, Color = ActionColor.Red }
                },
                Triggers.FinishTrigger,
                descriptionShort: "Sensitive Controls",
                descriptionLong: "You may perform a red barrel roll or boost action"
            );
        }
    }
}