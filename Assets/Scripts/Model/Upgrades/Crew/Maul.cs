using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;

namespace UpgradesList
{
    public class Maul : GenericUpgrade
    {
        public Maul() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Maul";
            Cost = 3;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum || ship.faction == Faction.Rebel;
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = false;

            if (squadList.SquadFaction == Faction.Scum)
            {
                result = true;
            }

            if (squadList.SquadFaction == Faction.Rebel)
            {
                foreach (var shipHolder in squadList.GetShips())
                {
                    if (shipHolder.Instance.PilotName == "Ezra Bridger")
                    {
                        return true;
                    }

                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetInstalledUpgrades())
                    {
                        if (upgrade.Name == "Ezra Bridger")
                        {
                            return true;
                        }
                    }
                }

                if (result != true)
                {
                    Messages.ShowError("Maul cannot be in Rebel squad without Ezra Bridger");
                }
                
            }
            
            return result;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += MaulDiceModification;
            host.OnAttackHitAsAttacker += RegisterRemoveStress;
        }

        private void MaulDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.MaulDiceModification(){ ImageUrl = ImageUrl, Host = this.Host };
            host.AddAvailableActionEffect(newAction);
        }

        private void RegisterRemoveStress()
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Maul: Remove Stress token",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnAttackHit,
                EventHandler = RemoveStress
            });
        }

        private void RemoveStress(object sender, System.EventArgs e)
        {
            if (Host.Tokens.HasToken(typeof(StressToken)))
            {
                Messages.ShowInfo("Maul: Remove Stress token");
                Host.Tokens.RemoveToken(
                    typeof(StressToken),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace ActionsList
{

    public class MaulDiceModification : GenericAction
    {

        public MaulDiceModification()
        {
            Name = EffectName = "Maul";

            IsReroll = true;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) && (!Combat.Attacker.Tokens.HasToken(typeof(StressToken)))) result = true;
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if ((Combat.AttackStep == CombatStep.Attack) && (!Combat.Attacker.Tokens.HasToken(typeof(StressToken))))
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Host.OnRerollIsConfirmed += AssignStressForEachRerolled;

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

        private void AssignStressForEachRerolled(GenericShip ship)
        {
            int diceRerolledCount = DiceRerollManager.CurrentDiceRerollManager.GetDiceReadyForReroll().Count();

            for (int i = 0; i < diceRerolledCount; i++)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Maul: Assign stress for each rerolled dice",
                    TriggerType = TriggerTypes.OnRerollIsConfirmed,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = AssingStress,
                    Skippable = true
                });
            }

            Host.OnRerollIsConfirmed -= AssignStressForEachRerolled;
        }

        private void AssingStress(object sender, System.EventArgs e)
        {
            Messages.ShowError("Maul: Get Stress token");
            Host.Tokens.AssignToken(new StressToken(Host), Triggers.FinishTrigger);
        }

    }

}
