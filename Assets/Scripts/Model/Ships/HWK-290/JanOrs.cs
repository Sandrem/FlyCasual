using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;
using Tokens;
using BoardTools;
using Arcs;
using RuleSets;

namespace Ship
{
    namespace HWK290
    {
        public class JanOrs : HWK290, ISecondEditionPilot
        {
            public JanOrs() : base()
            {
                PilotName = "Jan Ors";
                PilotSkill = 8;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Rebel;

                PilotAbilities.Add(new Abilities.JanOrsAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 42;

                PilotAbilities.RemoveAll(ability => ability is Abilities.JanOrsAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.JanOrsAbilitySE());

                SEImageNumber = 42;
            }
        }
    }
}

namespace Abilities
{
    public class JanOrsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += RegisterJanOrsAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= RegisterJanOrsAbility;
        }

        protected virtual void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId != HostShip.ShipId)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Combat.Attacker, HostShip);
                if (distanceInfo.Range < 4)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }

        protected void AskJanOrsAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                AskToUseAbility(AlwaysUseByDefault, UseJanOrsAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseJanOrsAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), AllowRollAdditionalDice);
        }

        private void AllowRollAdditionalDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += IncreaseByOne;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseByOne(ref int value)
        {
            value++;
            Combat.Attacker.AfterGotNumberOfAttackDice -= IncreaseByOne;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JanOrsAbilitySE : JanOrsAbility
    {
        protected override void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId != HostShip.ShipId 
                && Combat.ChosenWeapon.GetType() == typeof(Ship.PrimaryWeaponClass))
            {
                DistanceInfo distanceInfo = new DistanceInfo(Combat.Attacker, HostShip);
                if (distanceInfo.Range < 4 && Board.IsShipInArc(HostShip, Combat.Attacker))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }
    }
}