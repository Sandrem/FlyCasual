using Movement;
using Ship;
using System.Linq;

namespace Abilities
{
    public class AfterYouDefend : TriggerForAbility
    {
        private TriggeredAbility Ability;

        public int AttackRangeMin { get; }
        public int AttackRangeMax { get; }
        public bool OnlyIfAttackIsModifiedByAttacker { get; }

        public AfterYouDefend
        (
            int attackRangeMin = -1,
            int attackRangeMax = -1,
            bool onlyIfAttackIsModifiedByAttacker = false
        )
        {
            AttackRangeMin = attackRangeMin;
            AttackRangeMax = attackRangeMax;
            OnlyIfAttackIsModifiedByAttacker = onlyIfAttackIsModifiedByAttacker;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnAttackFinishAsDefender += CheckConditions;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnAttackFinishAsDefender -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship)
        {
            if (
                (AttackRangeMin == -1 || (Combat.ShotInfo.Range >= AttackRangeMin))
                && (AttackRangeMax == -1 || (Combat.ShotInfo.Range <= AttackRangeMax))
                && (!OnlyIfAttackIsModifiedByAttacker || Combat.DiceRollAttack.ModifiedByPlayers.Contains(Combat.Attacker.Owner.PlayerNo))
            )
            {
                Ability.RegisterAbilityTrigger
                (
                    TriggerTypes.OnAttackFinish,
                    delegate { Ability.Action.DoAction(Ability); }
                );
            }
        }
    }
}
