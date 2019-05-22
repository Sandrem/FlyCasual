using Upgrade;
using Ship;
using ActionsList;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class HanSoloScum : GenericUpgrade
    {
        public HanSoloScum() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Han Solo (Scum)",
                UpgradeType.Gunner,
                cost: 12,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.HanSoloScumGunnerAbility),
                restriction: new FactionRestriction(Faction.Scum),
                seImageNumber: 163
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloScumGunnerAbility : GenericAbility
    {
        // Before you engage, you may perform a red (Focus) action.

        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, UseAbility);
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": You may perform a free Red Focus action");

            HostShip.AskPerformFreeAction(new FocusAction() { Color = ActionColor.Red }, Triggers.FinishTrigger);
        }
    }
}