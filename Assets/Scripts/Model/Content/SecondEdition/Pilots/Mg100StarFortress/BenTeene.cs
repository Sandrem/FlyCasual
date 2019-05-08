using ActionsList;
using Arcs;
using BoardTools;
using Bombs;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class BenTeene : Mg100StarFortress
        {
            public BenTeene() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ben Teene",
                    3,
                    63,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BenTeeneAbility)
                );

                ModelInfo.SkinName = "Crimson";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/c9189c7e510b4d734d4d78c4f595010f.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BenTeeneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.ShotInfo.InArcByType(ArcType.SingleTurret))
            {
                Messages.ShowInfo("The \"Rattled\" condition has been assigned to " + Combat.Defender.PilotInfo.PilotName);
                Combat.Defender.Tokens.AssignCondition(
                    new Conditions.RattledCondition(Combat.Defender, HostShip)
                );
            }
        }
    }
}

namespace Conditions
{
    public class RattledCondition : GenericToken
    {
        public RattledCondition(GenericShip host, GenericShip source) : base(host)
        {
            Name = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }

        public override void WhenAssigned()
        {
            Host.OnGenerateActions += AddRepairAction;
            BombsManager.OnBombIsRemoved += CheckDamage;
        }

        public override void WhenRemoved()
        {
            Host.OnGenerateActions -= AddRepairAction;
            BombsManager.OnBombIsRemoved -= CheckDamage;
        }

        private void CheckDamage(GenericBomb bomb, GameObject model)
        {
            if (BombsManager.GetShipsInRange(model).Contains(Host))
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Rattled: Damage",
                        TriggerOwner = Host.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        EventHandler = RattledDamage
                    }
                );
            }
        }

        private void RattledDamage(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("A Critical Hit has been suffered because of the \"Rattled\" condition!");
            Host.Damage.TryResolveDamage(
                0,
                1,
                new DamageSourceEventArgs
                {
                    Source = this,
                    DamageType = DamageTypes.CardAbility
                },
                delegate
                {
                    Host.Tokens.RemoveCondition(this);
                    Triggers.FinishTrigger();
                }
            );
        }

        private void AddRepairAction(GenericShip ship)
        {
            GenericAction action = new RattledRepairAction()
            {
                HostShip = Host
            };
            Host.AddAvailableAction(action);
        }
    }
}

namespace ActionsList
{

    public class RattledRepairAction : GenericAction
    {
        public RattledRepairAction()
        {
            Name = DiceModificationName = "\"Rattled\" has been repaired. Discarding the \"Rattled\" condition.";
            ImageUrl = new Ship.SecondEdition.Mg100StarFortress.BenTeene().ImageUrl;
        }

        public override void ActionTake()
        {
            HostShip.Tokens.RemoveCondition(typeof(Conditions.RattledCondition));

            Phases.CurrentSubPhase.CallBack();
        }

        public override int GetActionPriority()
        {
            int result = 90;

            return result;
        }

        public override bool IsActionAvailable()
        {
            return BombsManager.GetBombsInRange(HostShip).Count == 0;
        }

    }

}