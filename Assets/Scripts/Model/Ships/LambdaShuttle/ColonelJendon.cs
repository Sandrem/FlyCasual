using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class ColonelJendon : LambdaShuttle
        {
            public ColonelJendon() : base()
            {
                PilotName = "Colonel Jendon";
                PilotSkill = 6;
                Cost = 26;
                IsUnique = true;

                PilotAbilities.Add(new Abilities.ColonelJendonAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ColonelJendonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterColonelJendonAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterColonelJendonAbility;
        }

        private void RegisterColonelJendonAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, StartSubphaseForColonelJendonAbility);            
        }
        
        private void StartSubphaseForColonelJendonAbility(object sender, System.EventArgs e)
        {
            if (HostShip.Owner.Ships.Count > 1 && HostShip.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*'))
            {
                var pilotAbilityDecision = (ColonelJendonDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(ColonelJendonDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                pilotAbilityDecision.InfoText = "Use Colonel Jendon's ability?";

                var blueTargetLocks = HostShip.Tokens.GetAllTokens()
                   .Where(t => t is Tokens.BlueTargetLockToken)
                   .Select(x => (Tokens.BlueTargetLockToken)x)
                   .OrderBy(y => y.Letter)
                   .ToList();

                pilotAbilityDecision.AddDecision("No", DontUseColonelJendonAbility);

                blueTargetLocks.ForEach(l => {
                    var name = "Target Lock " + l.Letter;
                    pilotAbilityDecision.AddDecision(name, delegate { UseColonelJendonAbility(l.Letter); });
                    pilotAbilityDecision.AddTooltip(name, l.OtherTokenOwner.ImageUrl);
                });

                pilotAbilityDecision.DefaultDecisionName = "No";
                pilotAbilityDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

                pilotAbilityDecision.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        
        private void UseColonelJendonAbility(char letter)
        {
            Tooltips.EndTooltip();

            SelectTargetForAbility(
                SelectColonelJendonAbilityTarget,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostShip.PilotName,
                "Choose a ship to assign to it one of your Blue Target Lock tokens if it does not have a Blue Target Lock token.",
                HostShip.ImageUrl
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 1);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            if (ship.Tokens.CountTokensByType(typeof(Tokens.BlueTargetLockToken)) == 0) result += 100;
            if (Actions.HasTarget(ship)) result += 50;
            if (ship.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.Types.Contains(Upgrade.UpgradeType.Missile) || n.Types.Contains(Upgrade.UpgradeType.Torpedo))) result += 25;

            return result;
        }

        private void SelectColonelJendonAbilityTarget()
        {
            if (TargetShip.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*'))
            {
                Messages.ShowErrorToHuman("Only ships without blue target lock tokens can be selected");
                return;
            }

            MovementTemplates.ReturnRangeRuler();

            var token = HostShip.Tokens.GetToken(typeof(Tokens.BlueTargetLockToken), '*') as Tokens.BlueTargetLockToken;

            Actions.ReassignTargetLockToken(
                token.Letter,
                this.HostShip,
                TargetShip,
                delegate{
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    DecisionSubPhase.ConfirmDecision();
                });
        }

        private void DontUseColonelJendonAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private class ColonelJendonDecisionSubPhase : DecisionSubPhase
        {
            public override void SkipButton()
            {
                ConfirmDecision();
            }
        }
    }
}
