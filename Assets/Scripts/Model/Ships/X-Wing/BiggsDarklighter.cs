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
                PilotSkill = 5;
                Cost = 25;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.BiggsDarklighterAbility());
            }
        }
    }
}

namespace Abilities
{
    public class BiggsDarklighterAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Phases.OnCombatPhaseStart += RegisterAskBiggsAbility;
        }

        private void RegisterAskBiggsAbility()
        {
            if (!IsAbilityUsed)
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
            IsAbilityUsed = true;
            HostShip.AssignToken(new Conditions.BiggsDarklighterCondition(), delegate { });

            GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;

            HostShip.OnDestroyed += RemoveBiggsDarklighterAbility;
            Phases.OnCombatPhaseEnd += RemoveBiggsDarklighterAbility;

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        public void CanPerformAttack(ref bool result, List<string> stringList)
        {
            bool shipIsProtected = false;
            if (Selection.AnotherShip.ShipId != HostShip.ShipId)
            {
                if (Selection.AnotherShip.Owner.PlayerNo == HostShip.Owner.PlayerNo)
                {
                    Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(Selection.AnotherShip, HostShip);
                    if (positionInfo.Range <= 1)
                    {
                        if (!Selection.ThisShip.ShipsBumped.Contains(HostShip))
                        {
                            if (Combat.ChosenWeapon.IsShotAvailable(HostShip)) shipIsProtected = true;
                        }
                    }
                }
            }

            if (shipIsProtected)
            {
                if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                {
                    stringList.Add("Biggs DarkLighter: You cannot attack target ship");
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
            HostShip.RemoveToken(typeof(Conditions.BiggsDarklighterCondition));

            GenericShip.OnTryPerformAttackGlobal -= CanPerformAttack;

            HostShip.OnDestroyed -= RemoveBiggsDarklighterAbility;
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