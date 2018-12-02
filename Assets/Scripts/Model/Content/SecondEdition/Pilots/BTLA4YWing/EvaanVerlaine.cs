using Abilities.SecondEdition;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class EvaanVerlaine : BTLA4YWing
        {
            public EvaanVerlaine() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Evaan Verlaine",
                    3,
                    36,
                    isLimited: true,
                    abilityType: typeof(EvaanVerlaineAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 16
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EvaanVerlaineAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += TryRegisterEvaanVerlaineAbiliity;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= TryRegisterEvaanVerlaineAbiliity;
        }

        private void TryRegisterEvaanVerlaineAbiliity()
        {
            if (TargetsForAbilityExist(FilterAbilityTargets) && HostShip.Tokens.HasToken<FocusToken>())
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                delegate
                {
                    HostShip.Tokens.SpendToken(typeof(FocusToken), IncreaseAgilityValue);
                },
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose a ship to increase it's agility value.",
                HostShip
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) && FilterTargetsByRange(ship, 1, 1);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            // I don't know how good of a metric this is?
            return ship.PilotInfo.Cost * ship.State.Agility;
        }

        protected void IncreaseAgilityValue()
        {
            TargetShip.ChangeAgilityBy(+1);
            Phases.Events.OnEndPhaseStart_NoTriggers += DecreaseAgilityValue;
            SelectShipSubPhase.FinishSelection();
        }

        protected void DecreaseAgilityValue()
        {
            TargetShip.ChangeAgilityBy(-1);
            Phases.Events.OnEndPhaseStart_NoTriggers -= DecreaseAgilityValue;
        }
    }
}
