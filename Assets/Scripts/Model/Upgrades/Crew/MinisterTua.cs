using Upgrade;
using Ship;
using GameModes;
using Abilities;
using Tokens;
using System;
using RuleSets;
using ActionsList;
using System.Collections.Generic;

namespace UpgradesList
{
    public class MinisterTua : GenericUpgrade, ISecondEditionUpgrade
    {
        public MinisterTua() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Minister Tua";
            Cost = 7;

            isUnique = true;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.MinisterTuaCrewAbility());

            SEImageNumber = 119;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }

    }
}

namespace Abilities.SecondEdition
{
    public class MinisterTuaCrewAbility : GenericAbility
    {
        //At the start of the Engagement Phase, if you are damaged, you may perform a red (reinforce) action.

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterReinforceIfDamagedAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterReinforceIfDamagedAbility;
        }

        private void RegisterReinforceIfDamagedAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, ReinforceIfDamagedAbility);
        }

        private void ReinforceIfDamagedAbility(object sender, System.EventArgs e)
        {
            if (HostShip.Damage.DamageCards.Count >= 1)
            {
                Selection.ThisShip = HostShip;
                HostShip.AskPerformFreeAction(new List<GenericAction> { new ReinforceAftAction() { Host = HostShip, IsRed = true }, new ReinforceForeAction() { Host = HostShip, IsRed = true } }, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}