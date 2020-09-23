using Movement;
using Ship;
using System.Linq;

namespace Abilities
{
    public class AfterYouAttack : TriggerForAbility
    {
        private TriggeredAbility Ability;

        public int AttackRangeMin { get; }
        public int AttackRangeMax { get; }
        public bool OnlyIfDefenseIsModifiedByDefender { get; }

        public AfterYouAttack
        (
            int attackRangeMin = -1,
            int attackRangeMax = -1,
            bool onlyIfDefenseIsModifiedByDefender = false
        )
        {
            AttackRangeMin = attackRangeMin;
            AttackRangeMax = attackRangeMax;
            OnlyIfDefenseIsModifiedByDefender = onlyIfDefenseIsModifiedByDefender;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnAttackFinishAsAttacker += CheckConditions;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnAttackFinishAsAttacker -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship)
        {
            if (
                (AttackRangeMin == -1 || (Combat.ShotInfo.Range >= AttackRangeMin))
                && (AttackRangeMax == -1 || (Combat.ShotInfo.Range <= AttackRangeMax))
                && (!OnlyIfDefenseIsModifiedByDefender || Combat.DiceRollDefence.ModifiedByPlayers.Contains(Combat.Defender.Owner.PlayerNo))
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
