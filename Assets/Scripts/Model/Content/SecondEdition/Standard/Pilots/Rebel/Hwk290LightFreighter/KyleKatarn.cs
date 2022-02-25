using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class KyleKatarn : Hwk290LightFreighter
        {
            public KyleKatarn() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Kyle Katarn",
                    "Relentless Operative",
                    Faction.Rebel,
                    3,
                    6,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KyleKatarnAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 43
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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

        protected virtual string GenerateAbilityString()
        {
            return "Choose another ship in arc to assign 1 of your Focus tokens to it";
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                Board.IsShipInArc(HostShip, ship);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            int shipRequiredTokens = ship.Tokens.CountTokensByType(GetTokenType());
            if (shipRequiredTokens == 0) result += 100;
            result += (5 - shipRequiredTokens);
            return result;
        }

        private void SelectAbilityTarget()
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
