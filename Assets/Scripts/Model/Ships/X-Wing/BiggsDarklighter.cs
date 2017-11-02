using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace XWing
    {
        public class BiggsDarklighter : XWing
        {
            public BiggsDarklighter() : base()
            {
                PilotName = "Biggs Darklighter";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/9/90/Biggs-darklighter.png";
                PilotSkill = 5;
                Cost = 25;

                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.BiggsDarklighterAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class BiggsDarklighterAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Phases.OnCombatPhaseStart += RegisterAskBiggsAbility;
        }

        private void RegisterAskBiggsAbility()
        {
            if (!isAbilityUsed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, ActivateBiggsAbility);
        }

        private void ActivateBiggsAbility(object sender, System.EventArgs e)
        {
            isAbilityUsed = true;
            Host.AssignToken(new Conditions.BiggsDarklighterCondition(), delegate { });

            RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal += CanPerformAttack;

            Host.OnDestroyed += RemoveBiggsDarklighterAbility;
            Phases.OnCombatPhaseEnd += RemoveBiggsDarklighterAbility;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        public void CanPerformAttack(ref bool result, GenericShip attacker, GenericShip defender)
        {
            bool shipIsProtected = false;
            if (defender.ShipId != Host.ShipId)
            {
                if (defender.Owner.PlayerNo == Host.Owner.PlayerNo)
                {
                    Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(defender, Host);
                    if (positionInfo.Range <= 1)
                    {
                        if (!attacker.ShipsBumped.Contains(Host))
                        {
                            if (Combat.ChosenWeapon.IsShotAvailable(Host)) shipIsProtected = true;
                        }
                    }
                }
            }

            if (shipIsProtected)
            {
                if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                {
                    Messages.ShowErrorToHuman("Biggs DarkLighter: You cannot attack target ship");
                }
                result = false;
            }
        }

        private void RemoveBiggsDarklighterAbility(GenericShip ship)
        {
            RemoveBiggsDarklighterAbility();
        }

        private void RemoveBiggsDarklighterAbility(object sender, System.EventArgs e)
        {
            RemoveBiggsDarklighterAbility();
        }

        private void RemoveBiggsDarklighterAbility()
        {
            Host.RemoveToken(typeof(Conditions.BiggsDarklighterCondition));

            RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal -= CanPerformAttack;

            Host.OnDestroyed -= RemoveBiggsDarklighterAbility;
            Phases.OnCombatPhaseEnd -= RemoveBiggsDarklighterAbility;

            Phases.OnCombatPhaseStart -= RegisterAskBiggsAbility;
        }
    }
}

namespace Conditions
{
    public class BiggsDarklighterCondition : Tokens.GenericToken
    {
        public BiggsDarklighterCondition()
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new Ship.XWing.BiggsDarklighter().ImageUrl;
        }
    }
}