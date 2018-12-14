using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Muse : TIEFoFighter
        {
            public Muse() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Muse\"",
                    2,
                    32,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MuseAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/8/81/Swz18_a1_muse.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MuseAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += TryRegisterMuseAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= TryRegisterMuseAbility;
        }

        protected virtual void TryRegisterMuseAbility()
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
            }
        }

        protected virtual void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                TryRemoveStress,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship to remove 1 Stress token from it.",
                HostShip
            );
        }

        protected bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(
                ship, new List<TargetTypes>() { TargetTypes.This, TargetTypes.OtherFriendly })
                && FilterTargetsByRange(ship, 0, 1);
        }

        protected int GetAiPriorityOfTarget(GenericShip ship)
        {
            return ship.Tokens.CountTokensByType(typeof(StressToken));
        }

        private void TryRemoveStress()
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            if (TargetShip.Tokens.HasToken<StressToken>())
            {
                TargetShip.Tokens.RemoveToken(typeof(StressToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}