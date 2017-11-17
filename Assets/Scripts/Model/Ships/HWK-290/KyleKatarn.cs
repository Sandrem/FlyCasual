using Ship;
using SubPhases;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace HWK290
    {
        public class KyleKatarn : HWK290
        {
            public KyleKatarn() : base()
            {
                PilotName = "Kyle Katarn";
                PilotSkill = 6;
                Cost = 21;

                faction = Faction.Rebel;

                PilotAbilities.Add(new PilotAbilitiesNamespace.KyleKatarnAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class KyleKatarnAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Phases.OnCombatPhaseStart += RegisterAbility;
            Host.OnDestroyed += RemoveAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        private void RemoveAbility(GenericShip ship)
        {
            Phases.OnCombatPhaseStart -= RegisterAbility;
        }

        private void Ability(object sender, EventArgs e)
        {
            if (Host.Owner.Ships.Count > 1 && Host.HasToken(typeof(Tokens.FocusToken)))
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    new List<TargetTypes> { TargetTypes.OtherFriendly },
                    new UnityEngine.Vector2(1, 3));
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectAbilityTarget()
        {
            Host.RemoveToken(typeof(Tokens.FocusToken));
            TargetShip.AssignToken(new Tokens.FocusToken(), SelectShipSubPhase.FinishSelection);           
        }
    }
}
