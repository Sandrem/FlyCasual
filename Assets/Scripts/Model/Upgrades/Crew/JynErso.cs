using Abilities;
using DamageDeckCard;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using Conditions;
using UnityEngine;

namespace UpgradesList
{
    public class JynErso : GenericUpgrade
    {
        public JynErso() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Jyn Erso";
            Cost = 2;

            isUnique = true;

            AvatarOffset = new Vector2(68, 0);

            UpgradeAbilities.Add(new JynErsoAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class JynErsoAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += JynErsoAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= JynErsoAddAction;
        }

        private void JynErsoAddAction(GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.JynErsoAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip,
                DoAction = DoJynErsoAction
            };
            host.AddAvailableAction(action);
        }

        private void DoJynErsoAction()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, SelectShip);

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Phases.CurrentSubPhase.CallBack);
        }

        private void SelectShip(object sender, EventArgs e)
        {
            // TODO: Skip/Wrong target - revert

            SelectTargetForAbility(
                AssignFocusTokensToTarget,
                FilterTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostUpgrade.Name,
                "Choose a ship. Assign 1 Focus token to that ship for each enemy ship inside your firing arc.",
                HostUpgrade.ImageUrl
            );
        }

        private void AssignFocusTokensToTarget()
        {
            // Count ships in arc of Jyn Erso's ship...
            var tokenCount = Roster.AllShips.Values
                .Where(s => s.Owner.Id != HostShip.Owner.Id)
                .Where(s =>
                {
                    var arcInfo = new Board.ShipShotDistanceInformation(HostShip, s);
                    return arcInfo.InArc && arcInfo.Range <= 3;
                })
                .Count();
            // ... to a maximum of 3...
            tokenCount = Math.Min(tokenCount, 3);
            
            // ... and assign that many focus tokens to the selected ship
            Messages.ShowInfo(string.Format("{0} assigns {1} focus {3} to {2}.", HostUpgrade.Name, tokenCount, TargetShip.PilotName, tokenCount == 1 ? "token" : "tokens"));
            if (tokenCount > 0)
            {                
                // Assign the tokens
                RegisterAssignMultipleFocusTokens(tokenCount);
                // Jyn says something
                var clip = new[] { "JynErso1", "JynErso2", "JynErso3", "JynErso4", "JynErso5" }[UnityEngine.Random.Range(0, 5)];                    
                Sounds.PlayShipSound(clip);
            }
        }

        private void RegisterAssignMultipleFocusTokens(int count)
        {
            for (var i = 0; i < count; i++)
            {
                Selection.ThisShip = TargetShip;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, (s, e) =>
                {
                    TargetShip.Tokens.AssignToken(new Tokens.FocusToken(TargetShip), () =>
                    {
                        Selection.ThisShip = HostShip;
                        Phases.CurrentSubPhase.Resume();
                        Triggers.FinishTrigger();
                    });
                });
            }
            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new[] { TargetTypes.OtherFriendly, TargetTypes.This }.ToList()) && FilterTargetsByRange(ship, 1, 2);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            result += NeedTokenPriority(ship);
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);
            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) return 100;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.EvadeAction)) && !ship.Tokens.HasToken(typeof(Tokens.EvadeToken))) return 50;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.TargetLockAction)) && !ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        
    }
}

namespace ActionsList
{
    public class JynErsoAction : GenericAction
    {
        public JynErsoAction()
        {
            Name = EffectName = "Jyn Erso";
        }

        protected bool AreThereEnemiesInArc
        {
            get
            {
                return Roster.AllShips.Values
                    .Where(s => s.Owner.Id != Host.Owner.Id)
                    .Where(s =>
                    {
                        var arcInfo = new Board.ShipShotDistanceInformation(Host, s);
                        return arcInfo.InArc && arcInfo.Range <= 3;
                    })
                    .Any();
            }
        }

        public override int GetActionPriority()
        {            
            return 50;
        }

        public override bool IsActionAvailable()
        {
            // Let's not lead the player to use the action if they will get no benefit out of it
            return AreThereEnemiesInArc;
        }
    }
}
