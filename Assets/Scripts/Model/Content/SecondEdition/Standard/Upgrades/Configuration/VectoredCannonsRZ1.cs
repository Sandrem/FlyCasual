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
    public class VectoredCannonsRZ1 : GenericUpgrade
    {
        public VectoredCannonsRZ1() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Vectored Сannons (RZ-1)",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new AbilityPresenceRestriction(typeof(Abilities.SecondEdition.VectoredThrustersAbility)),
                addArc: new ShipArcInfo(ArcType.SingleTurret, 2),
                removeArc: ArcType.Front,
                abilityType: typeof(Abilities.SecondEdition.VectoredCannonsRZ1Ability),
                isStandardazed: true
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b0/1a/b01a4dff-267b-436c-a719-878335302bca/swz83_upgrade_vectoredcannonsrz1.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VectoredCannonsRZ1Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            VectoredThrustersAbility oldAbility = (VectoredThrustersAbility) HostShip.ShipAbilities.First(n => n.GetType() == typeof(VectoredThrustersAbility));
            oldAbility.DeactivateAbility();
            HostShip.ShipAbilities.Remove(oldAbility);

            GenericAbility ability = new VectoredCannonsAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }

        public override void DeactivateAbility()
        {
            HostShip.ShipAbilities.RemoveAll(n => n.GetType() == typeof(VectoredCannonsRZ1Ability));

            GenericAbility ability = new VectoredThrustersAbility();
            ability.HostUpgrade = HostUpgrade;
            HostShip.ShipAbilities.Add(ability);
            ability.Initialize(HostShip);
        }
    }

    public class VectoredCannonsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;

            HostShip.OnGetAvailableArcFacings += RestrictArcFacings;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;

            HostShip.OnGetAvailableArcFacings -= RestrictArcFacings;
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
                    new BoostAction(){ HostShip = HostShip, Color = ActionColor.Red },
                    new RotateArcAction(){ HostShip = HostShip, Color = ActionColor.Red }
                },
                Triggers.FinishTrigger,
                descriptionShort: "Vectored Cannons",
                descriptionLong: "You may perform a red boost or rotate arc action",
                imageHolder: HostUpgrade
            );
        }

        private void RestrictArcFacings(List<ArcFacing> facings)
        {
            facings.Remove(ArcFacing.Left);
            facings.Remove(ArcFacing.Right);
        }
    }
}