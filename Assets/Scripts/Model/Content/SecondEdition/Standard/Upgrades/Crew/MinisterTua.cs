using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using ActionsList;
using Actions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class MinisterTua : GenericUpgrade
    {
        public MinisterTua() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Minister Tua",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.MinisterTuaCrewAbility),
                seImageNumber: 119
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(416, 1),
                new Vector2(125, 125)
            );
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
                HostShip.AskPerformFreeAction(
                    new ReinforceAction() { HostShip = HostShip, Color = ActionColor.Red },
                    Triggers.FinishTrigger,
                    HostUpgrade.UpgradeInfo.Name,
                    "At the start of the Engagement Phase, if you are damaged, you may perform a red Reinforce action",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}