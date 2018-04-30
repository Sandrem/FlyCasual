using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;
using System;
using Abilities;

namespace UpgradesList
{
    public class Maul : GenericUpgrade
    {
        public Maul() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Maul";
            Cost = 3;

            isUnique = true;

            AvatarOffset = new Vector2(59, 0);

            UpgradeAbilities.Add(new MaulCrewAbility());
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

                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
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

    }
}

namespace Abilities
{
    public class MaulCrewAbility: GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += MaulDiceModification;
            HostShip.OnAttackHitAsAttacker += RegisterRemoveStress;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= MaulDiceModification;
            HostShip.OnAttackHitAsAttacker -= RegisterRemoveStress;
        }

        private void MaulDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.MaulDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            HostShip.AddAvailableActionEffect(newAction);
        }

        private void RegisterRemoveStress()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, RemoveStress);
        }

        private void RemoveStress(object sender, System.EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                Messages.ShowInfo("Maul: Remove Stress token");
                HostShip.Tokens.RemoveToken(
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

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Maul: Assign stress for each rerolled dice",
                TriggerType = TriggerTypes.OnRerollIsConfirmed,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = delegate { StartAssignStess(diceRerolledCount); }
            });

            Host.OnRerollIsConfirmed -= AssignStressForEachRerolled;
        }

        private void StartAssignStess(int diceRerolledCount)
        {
            Messages.ShowError(string.Format("Maul: Get Stress tokens: {0}", diceRerolledCount));
            AssignStressRecursive(diceRerolledCount);
        }

        private void AssignStressRecursive(int count)
        {
            if (count > 0)
            {
                count--;
                Host.Tokens.AssignToken(new StressToken(Host), delegate { AssignStressRecursive(count); });
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }

}
