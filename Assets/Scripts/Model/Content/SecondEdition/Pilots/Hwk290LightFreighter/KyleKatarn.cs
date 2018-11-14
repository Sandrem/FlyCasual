using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class KyleKatarn : Hwk290LightFreighter
        {
            public KyleKatarn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kyle Katarn",
                    3,
                    38,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.KyleKatarnAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 43;
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
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        protected virtual string GenerateAbilityString()
        {
            return "Choose another ship to assign 1 of your Focus tokens to it.";
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Count > 1 && HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotName,
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
            int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(FocusToken));
            if (shipFocusTokens == 0) result += 100;
            result += (5 - shipFocusTokens);
            return result;
        }

        protected virtual void SelectAbilityTarget()
        {
            HostShip.Tokens.RemoveToken(
                typeof(FocusToken),
                delegate {
                    TargetShip.Tokens.AssignToken(typeof(FocusToken), SelectShipSubPhase.FinishSelection);
                }
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KyleKatarnAbility : Abilities.FirstEdition.KyleKatarnAbility
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                Board.IsShipInArc(HostShip, ship);
        }
    }
}
