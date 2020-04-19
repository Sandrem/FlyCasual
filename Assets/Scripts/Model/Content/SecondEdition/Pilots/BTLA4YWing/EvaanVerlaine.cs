using Abilities.SecondEdition;
using Conditions;
using Ship;
using SubPhases;
using System;
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
                    35,
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
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                delegate
                {
                    HostShip.Tokens.SpendToken(typeof(FocusToken), IncreaseAgilityValue);
                },
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a ship to protect",
                HostShip
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) && FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        protected void IncreaseAgilityValue()
        {
            EvaanVerlaineCondition condition = new EvaanVerlaineCondition(TargetShip, HostShip);
            TargetShip.Tokens.AssignCondition(condition);

            SelectShipSubPhase.FinishSelection();
        }
    }
}

namespace Conditions
{
    public class EvaanVerlaineCondition : GenericToken
    {
        private GenericShip AbilitySourceShip;

        public EvaanVerlaineCondition(GenericShip host, GenericShip source) : base(host)
        {
            Name = ImageName = "Buff Token";
            TooltipType = source.GetType();
            Temporary = false;
            AbilitySourceShip = source;
        }

        public override void WhenAssigned()
        {
            Messages.ShowInfo(AbilitySourceShip.PilotInfo.PilotName + " protects " + Host.PilotInfo.PilotName);

            Host.AfterGotNumberOfDefenceDice += IncreaseNumber;

            Phases.Events.OnEndPhaseStart_NoTriggers += RemoveThisCondition;
        }

        private void IncreaseNumber(ref int count)
        {
            Messages.ShowInfo(AbilitySourceShip.PilotInfo.PilotName + "'s protection: " + Host.PilotInfo.PilotName + " rolls 1 additional defense die");
            count++;
        }

        public void RemoveThisCondition()
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            Host.AfterGotNumberOfDefenceDice -= IncreaseNumber;

            Phases.Events.OnEndPhaseStart_NoTriggers += RemoveThisCondition;
        }
    }
}
