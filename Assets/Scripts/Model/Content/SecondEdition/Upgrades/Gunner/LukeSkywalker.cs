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
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                "Luke Skywalker",
                NeverUseByDefault,
                UseLukeSkywalkerGunnerAbility,
                descriptionLong: "Do you want to spend 1 Force to rotate your turret arc indicator?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void UseLukeSkywalkerGunnerAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.State.Force--;
            new RotateArcAction().DoOnlyEffect(Triggers.FinishTrigger);
        }
    }
}