using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using System;
using SubPhases;
using Tokens;
using Ship;

namespace Ship
{
    namespace JumpMaster5000
    {
        public class Manaroo : JumpMaster5000
        {
            public Manaroo() : base()
            {
                PilotName = "Manaroo";
                PilotSkill = 4;
                Cost = 27;

                IsUnique = true;

                // Already have Elite icon from JumpMaster5000 class

                PilotAbilities.Add(new ManarooAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ManarooAbility : GenericAbility
    {
        private static List<Type> ReassignableTokenTypes = new List<Type>()
        {
            typeof(FocusToken),
            typeof(EvadeToken),
            typeof(BlueTargetLockToken),
            typeof(RedTargetLockToken)
        };

        private List<GenericToken> ManarooTokens;

        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.Owner.Ships.Count == 1) return;

            if (CountFriendlyShipsRange1() == 1) return;

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectTarget);
        }

        private void SelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbilityOld(
                TargetToReassignIsSelected,
                new List<TargetTypes>() { TargetTypes.OtherFriendly },
                new Vector2(1, 1),
                null,
                true
            );
        }

        private void TargetToReassignIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo(string.Format("{0} : all Focus, Evade and Target Lock tokens reassigned to {1}", HostShip.PilotName, TargetShip.PilotName));

            ManarooTokens = new List<GenericToken>(HostShip.Tokens.GetAllTokens());
            ReassignTokensRecursive();
        }

        private void ReassignTokensRecursive()
        {
            GenericToken tokenToReassign = GetTokenOfSupportedType();

            if (tokenToReassign != null)
            {
                Actions.ReassignToken(tokenToReassign, HostShip, TargetShip, ReassignTokensRecursive);
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

        private int CountFriendlyShipsRange1()
        {
            return Board.BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 1), Team.Type.Friendly).Count;
        }
    }
}