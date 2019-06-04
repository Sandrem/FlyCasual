using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.JumpMaster5000
    {
        public class Manaroo : JumpMaster5000
        {
            public Manaroo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Manaroo",
                    4,
                    27,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ManarooAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public abstract class ManarooCommonAbility : GenericAbility
    {
        protected abstract int MinRange { get; }

        protected abstract int MaxRange { get; }

        protected abstract List<Type> ReassignableTokenTypes { get; }

        protected abstract string SelectTargetMessage { get; }

        protected abstract string ReassignTokensMessage { get; }
    }

    //At the start of the Combat phase, you may assign all focus, evade, and target lock tokens assigned to you to another friendly ship at Range 1.
    public class ManarooAbility : ManarooCommonAbility
    {
        protected override int MinRange
        {
            get
            {
                return 1;
            }
        }

        protected override int MaxRange
        {
            get
            {
                return 1;
            }
        }

        protected override List<Type> ReassignableTokenTypes
        {
            get
            {
                return new List<Type>()
                {
                    typeof(FocusToken),
                    typeof(EvadeToken),
                    typeof(BlueTargetLockToken),
                    typeof(RedTargetLockToken)
                };
            }
        }

        protected override string SelectTargetMessage
        {
            get
            {
                return "Choose another friendly ship to assign all your focus, evade and target lock tokens to it";
            }
        }

        protected override string ReassignTokensMessage
        {
            get
            {
                return string.Format("{0}: all Focus, Evade and Target Lock tokens have been reassigned to {1}", HostShip.PilotInfo.PilotName, TargetShip.PilotInfo.PilotName);
            }
        }

        private List<GenericToken> ManarooTokens;

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        protected virtual void CheckAbility()
        {
            if (HostShip.Owner.Ships.Count == 1) return;

            if (CountFriendlyShipsInRange() == 0) return;

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectTarget);
        }

        protected void SelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                TargetToReassignIsSelected,
                FilterAbilityTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                SelectTargetMessage,
                HostShip
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, MinRange, MaxRange);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int result = 0;

            if (ship.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.GetType() == typeof(UpgradesList.FirstEdition.AttanniMindlink))) result += 9000;

            if (HostShip.Tokens.HasToken(typeof(FocusToken)) && !ship.Tokens.HasToken(typeof(FocusToken))) result += 500;
            if (HostShip.Tokens.HasToken(typeof(EvadeToken)) && !ship.Tokens.HasToken(typeof(EvadeToken))) result += 400;
            if (HostShip.Tokens.HasToken(typeof(BlueTargetLockToken), '*') && !ship.Tokens.HasToken(typeof(FocusToken), '*')) result += 300;

            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private void TargetToReassignIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo(ReassignTokensMessage);

            ManarooTokens = new List<GenericToken>(HostShip.Tokens.GetAllTokens());
            ReassignTokensRecursive();
        }

        private void ReassignTokensRecursive()
        {
            GenericToken tokenToReassign = GetTokenOfSupportedType();

            if (tokenToReassign != null)
            {
                ActionsHolder.ReassignToken(tokenToReassign, HostShip, TargetShip, ReassignTokensRecursive);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private GenericToken GetTokenOfSupportedType()
        {
            GenericToken supportedToken = ManarooTokens.Find(n => ReassignableTokenTypes.Contains(n.GetType()));

            if (supportedToken != null)
            {
                ManarooTokens.Remove(supportedToken);
            }

            return supportedToken;
        }

        protected int CountFriendlyShipsInRange()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Friendly).Count;
        }
    }
}
