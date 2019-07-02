using Actions;
using ActionsList;
using Ship;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class LukeSkywalker : GenericUpgrade
    {
        public LukeSkywalker() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Luke Skywalker",
                UpgradeType.Gunner,
                cost: 26,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.LukeSkywalkerGunnerAbility),
                seImageNumber: 98
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class LukeSkywalkerGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckLukeAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckLukeAbility;
        }

        private void CheckLukeAbility()
        {
            if (HostShip.State.Force > 0 && HostShip.ActionBar.HasAction(typeof(RotateArcAction)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToRotateArc);
            }
        }

        private void AskToRotateArc(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += SpendForce;
            Selection.ChangeActiveShip(HostShip);

            //TODO: Card states that you can rotate your arc, not perform a rotate arc action so you can do it while stressed
            HostShip.AskPerformFreeAction(
                new RotateArcAction() { Color = ActionColor.White, CanBePerformedWhileStressed = true },
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                "At the start of the Engagement Phase, you may spend 1 Force to rotate your Turret Arc indicator",
                HostUpgrade
            );
        }

        private void SpendForce(GenericAction action, ref bool isFreeAction)
        {
            HostShip.State.Force--;
            HostShip.BeforeActionIsPerformed -= SpendForce;
        }
    }
}