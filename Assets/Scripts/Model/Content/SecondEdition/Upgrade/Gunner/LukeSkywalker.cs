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
                cost: 30,
                isLimited: true,
                restrictionFaction: Faction.Rebel,
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
            HostShip.State.MaxForce += 1;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckLukeAbility;
            HostShip.State.MaxForce -= 1;
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
            Messages.ShowInfo("Luke Skywalker: You may spend 1 force to rotate your arc.");
            HostShip.BeforeFreeActionIsPerformed += SpendForce;
            Selection.ChangeActiveShip(HostShip);
            //Card states that you can rotate your arc, not perform a rotate arc action so you can do it while stressed
            HostShip.AskPerformFreeAction(new RotateArcAction() { IsRed = false, CanBePerformedWhileStressed = true }, Triggers.FinishTrigger);
        }

        private void SpendForce(GenericAction action)
        {
            HostShip.State.Force--;
            HostShip.BeforeFreeActionIsPerformed -= SpendForce;
        }
    }
}