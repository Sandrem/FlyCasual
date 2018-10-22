using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;
using RuleSets;
using SubPhases;
using Conditions;

namespace Ship
{
    namespace TIEFighter
    {
        public class CaptainRex : TIEFighter, ISecondEditionPilot
        {
            public CaptainRex() : base()
            {
                PilotName = "Captain Rex";
                PilotSkill = 2;
                Cost = 32;

                IsUnique = true;

                faction = Faction.Rebel;

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.CaptainRexPilotAbilitySE());

                SpecialModel = "TIE Fighter Rebel";
                SkinName = "Rebel";

                SEImageNumber = 48;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainRexPilotAbilitySE : GenericAbility
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
            Messages.ShowInfo("Suppressive Fire is assigned by Captain Rex");

            AssignedCondition = new CaptainRexCondition(Combat.Defender) { Source = HostShip };
            SufferedShip = Combat.Defender;
            SufferedShip.Tokens.AssignCondition(AssignedCondition);

            AttackedThisTurn = true;
        }

        private void RemoveCondition()
        {
            if (SufferedShip != null)
            {
                Messages.ShowInfo("Suppressive Fire is removed from " + SufferedShip.PilotName);

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
            TooltipType = typeof(Ship.TIEFighter.CaptainRex);

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
                Messages.ShowError("Captain Rex: Roll 1 fewer attack die");
                count--;
            }
        }
    }
}
