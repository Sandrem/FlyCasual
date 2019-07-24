
using Abilities.SecondEdition;
using BoardTools;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;
using Conditions;
using System.Linq;
using ActionsList;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class NabooHandmaiden : NabooRoyalN1Starfighter
        {
            public NabooHandmaiden() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Naboo Handmaiden",
                    1,
                    44,
                    limited: 2,
                    abilityText: "Setup: After placing forces, assign the Decoyed condition to 1 friendly ship other than Naboo Handmaiden.",
                    abilityType: typeof(NabooHandmaidenAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
                // using UnityEngine;
                // Avatar = new AvatarInfo(Faction.Imperial, new Vector2(71, 1));
                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ad/b6/adb64448-5777-4fd3-8311-293207d7103b/swz40_naboo-handmaiden.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NabooHandmaidenAbility : GenericAbility
    {
        protected virtual string Prompt
        {
            get
            {
                return "Assign the Decoyed condition to 1 friendly ship other than Naboo Handmaiden.";
            }
        }
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterNabooHandmaidenAbility;
            // HostShip.OnAttackStartAsDefender += CheckNabooHandmaidenAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterNabooHandmaidenAbility;
            // HostShip.OnDefenceStartAsAttacker -= CheckNabooHandmaidenAbility;
            // HostShip.OnAttackStartAsDefender -= CheckNabooHandmaidenAbility;
        }
        private void RegisterNabooHandmaidenAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Naboo Handmaiden decision",
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectNabooHandmaidenTarget,
            });
        }

        private void SelectNabooHandmaidenTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignDecoyed,
                  CheckRequirements,
                  GetAiDecoyedPriority,
                  HostShip.Owner.PlayerNo,
                  "Decoyed",
                  Prompt,
                  HostUpgrade
            );
        }

        protected virtual void AssignDecoyed()
        {
            TargetShip.Tokens.AssignCondition(new Decoyed(TargetShip) { SourceUpgrade = HostUpgrade });
            SelectShipSubPhase.FinishSelection();
        }

        protected virtual bool CheckRequirements(GenericShip ship)
        {
            var match = ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.PilotInfo.PilotName != "Naboo Handmaiden";
            return match;
        }

        private int GetAiDecoyedPriority(GenericShip ship)
        {
            int result = 0;
            int isN1 = 0;
            if (ship.ShipInfo.ShipName == "Naboo Royal N-1 Starfighter") isN1 = 1;

            result += (ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost))*(1+isN1);

            return result;
        }        
    }
}

namespace Conditions
{
    public class Decoyed : GenericToken
    {
        public GenericUpgrade SourceUpgrade;

        public Decoyed(GenericShip host) : base(host)
        {
            Name = ImageName = "Decoyed Condition";
            Temporary = false;
            // Tooltip = "https://raw.githubusercontent.com/Sandrem/xwing-data2-test/master/images/conditions/decoyed.png";
            Tooltip = "https://images-cdn.fantasyflightgames.com/filer_public/7e/38/7e38aca8-b0ea-4ddc-8ec4-64efca1544c8/swz40_decoyed.png";
        }

        public override void WhenAssigned()
        {
            SubscribeToDecoyedConditionEffects();
        }

        public override void WhenRemoved()
        {
            UnsubscribeFromDecoyedConditionEffects();
        }

        private void SubscribeToDecoyedConditionEffects()
        {
            // Remove any other Decoy conditions on friendly ships
            Host.OnGenerateDiceModifications += AddDecoyedResultModification;
        }

        private void UnsubscribeFromDecoyedConditionEffects()
        {
            Host.OnGenerateDiceModifications -= AddDecoyedResultModification;
        }

        private void AddDecoyedResultModification(GenericShip ship)
        {
            DecoyedAction action = new DecoyedAction()
            {
                HostShip = Host,
                SourceUpgrade = SourceUpgrade
            };
            Host.AddAvailableDiceModification(action);    
        }
    }
}

namespace ActionsList
{
    public class DecoyedAction : GenericAction
    {
        public GenericUpgrade SourceUpgrade;

        public DecoyedAction()
        {
            Name = DiceModificationName = "Decoyed";
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            // mod only works on defense
            if (HostShip != Combat.Defender || Combat.AttackStep != CombatStep.Defence) result = false;

            // handmaidens in attack arc with evades to spend
            int handmaidens = Roster.AllShips.Count(v =>
                v.Value.PilotInfo.PilotName == "Naboo Handmaiden"
                && new ShotInfo(Combat.Attacker, v.Value, Combat.Attacker.PrimaryWeapons).InArc
                && v.Value.Tokens.HasToken(typeof(EvadeToken))
            );
            if (handmaidens == 0) result = false;
            Messages.ShowInfo("Handmaidens in Range: " + handmaidens);
            return result;
        }

        private class OptimizedPrototypeDecisionSubPhase : DecisionSubPhase { }

        public override void ActionEffect(System.Action callBack)
        {
            var newSubPhase = Phases.StartTemporarySubPhaseNew<OptimizedPrototypeDecisionSubPhase>(Name, callBack);

            newSubPhase.DescriptionShort = "Naboo Handmaiden Decoy";
            newSubPhase.DescriptionLong = "Do you want to spend any evades from friendly Naboo Handmaidens?";
            newSubPhase.ImageSource = SourceUpgrade;

            newSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            newSubPhase.ShowSkipButton = true;
            newSubPhase.OnSkipButtonIsPressed = DontUseOptimizedPrototype;

            var validHandmaidens = Roster.AllShips.Where(v => v.Value.PilotInfo.PilotName == "Naboo Handmaiden"
                && new ShotInfo(Combat.Attacker, v.Value, Combat.Attacker.PrimaryWeapons).InArc
                && v.Value.Tokens.HasToken(typeof(EvadeToken))
            );

            foreach(var kvp in validHandmaidens)
            {
                GenericShip handmaiden = kvp.Value;
                newSubPhase.AddDecision(
                    "Naboo Handmaiden " + handmaiden.ShipId, (s, o) => handmaiden.Tokens.SpendToken(
                        typeof(EvadeToken), () => SpendOrAddEvade(handmaiden)
                    )
                );
            }

            newSubPhase.DefaultDecisionName = newSubPhase.GetDecisions().Select(d => d.Name).FirstOrDefault();
            newSubPhase.Start();
        }

        private void SpendOrAddEvade(GenericShip handmaiden)
        {
            if (HostShip.ShipInfo.ShipName == "Naboo Royal N-1 Starfighter")
            {
                Messages.ShowInfo("Naboo Handmaiden: added evade");
                Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
            }
            else
            {
                Combat.CurrentDiceRoll.ApplyEvade();
            }
        }

        private void DefenderSuffersDamage()
        {
            Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Damage from Optimized Prototype",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Combat.Defender.Owner.PlayerNo,
                    EventHandler = Combat.Defender.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = Combat.Attacker,
                        DamageType = DamageTypes.CardAbility
                    },
                    Skippable = true
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
        }

        private void DontUseOptimizedPrototype()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (IsDiceModificationAvailable())
            {
                if (Combat.DiceRollAttack.Blanks > 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 30;
                }
            }

            return result;
        }
    }
}