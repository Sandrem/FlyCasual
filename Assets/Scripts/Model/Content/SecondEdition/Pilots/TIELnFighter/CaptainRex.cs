using Conditions;
using Ship;
using Tokens;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class CaptainRex : TIELnFighter
        {
            public CaptainRex() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Rex",
                    2,
                    32,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainRexPilotAbility),
                    factionOverride: Faction.Rebel,
                    seImageNumber: 48
                );

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainRexPilotAbility : GenericAbility
    {
        private CaptainRexCondition AssignedCondition;
        private GenericShip SufferedShip;
        private bool AttackedThisTurn;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += AssignConditionToDefender;

            HostShip.OnAttackStartAsDefender += RemoveCondition;
            Phases.Events.OnCombatPhaseEnd_NoTriggers += CheckAttacked;
            HostShip.OnShipIsDestroyed += RemoveConditionOfShip;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= AssignConditionToDefender;

            HostShip.OnAttackStartAsDefender -= RemoveCondition;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= CheckAttacked;
            HostShip.OnShipIsDestroyed -= RemoveConditionOfShip;
        }

        private void AssignConditionToDefender(GenericShip ship)
        {
            Messages.ShowInfo("Suppressive Fire has been assigned by Captain Rex.");

            AssignedCondition = new CaptainRexCondition(Combat.Defender) { Source = HostShip };
            SufferedShip = Combat.Defender;
            SufferedShip.Tokens.AssignCondition(AssignedCondition);

            AttackedThisTurn = true;
        }

        private void RemoveCondition()
        {
            if (SufferedShip != null)
            {
                Messages.ShowInfo("Suppressive Fire has been removed from " + SufferedShip.PilotInfo.PilotName);

                SufferedShip.Tokens.RemoveCondition(AssignedCondition);
                SufferedShip = null;
                AssignedCondition = null;
            }
        }

        private void RemoveConditionOfShip(GenericShip ship, bool isForced)
        {
            RemoveCondition();
        }

        private void CheckAttacked()
        {
            if (!AttackedThisTurn) RemoveCondition();
            AttackedThisTurn = false;
        }
    }
}

namespace Conditions
{
    public class CaptainRexCondition : GenericToken
    {
        public GenericShip Source;

        public CaptainRexCondition(GenericShip host) : base(host)
        {
            Name = "Debuff Token";
            TooltipType = typeof(Ship.SecondEdition.TIELnFighter.CaptainRex);

            Temporary = false;
        }

        public override void WhenAssigned()
        {
            Host.AfterGotNumberOfAttackDice += CheckAbility;
        }

        public override void WhenRemoved()
        {
            Host.AfterGotNumberOfAttackDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (Combat.Defender != Source)
            {
                Messages.ShowError("Captain Rex - Suppressive Fire: Since the attacker is not attacking Captain Rex, it rolls 1 fewer attack die.");
                count--;
            }
        }
    }
}
