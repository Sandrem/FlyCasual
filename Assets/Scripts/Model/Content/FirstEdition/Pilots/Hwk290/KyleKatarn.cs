using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Hwk290
    {
        public class KyleKatarn : Hwk290
        {
            public KyleKatarn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kyle Katarn",
                    6,
                    21,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KyleKatarnAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KyleKatarnAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, DoAbility);
        }

        protected virtual string GenerateAbilityString()
        {
            return "Choose another ship to assign 1 of your Focus tokens to it";
        }

        protected virtual Type GetTokenType()
        {
            return typeof(FocusToken);
        }

        private void DoAbility(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Count > 1 && HostShip.Tokens.HasToken(GetTokenType()))
            {
                SelectTargetForAbility
                (
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    GenerateAbilityString(),
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            int shipRequiredTokens = ship.Tokens.CountTokensByType(GetTokenType());
            if (shipRequiredTokens == 0) result += 100;
            result += (5 - shipRequiredTokens);
            return result;
        }

        protected virtual void SelectAbilityTarget()
        {
            HostShip.Tokens.RemoveToken(
                GetTokenType(),
                delegate {
                    TargetShip.Tokens.AssignToken(GetTokenType(), SelectShipSubPhase.FinishSelection);
                }
            );
        }
    }
}