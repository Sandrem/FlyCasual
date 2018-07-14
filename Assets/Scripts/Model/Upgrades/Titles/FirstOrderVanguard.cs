using Ship;
using Ship.TIESilencer;
using Upgrade;
using Abilities;
using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using ActionsList;
using BoardTools;

namespace UpgradesList
{
    public class FirstOrderVanguard : GenericUpgrade
    {
        public FirstOrderVanguard() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "First Order Vanguard";
            Cost = 2;

            isUnique = true;

            UpgradeAbilities.Add(new FirstOrderVanguardAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIESilencer;
        }
    }
}

namespace Abilities
{
    public class FirstOrderVanguardAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += FirstOrderVanguardActionEffects;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= FirstOrderVanguardActionEffects;
        }

        private void FirstOrderVanguardActionEffects(GenericShip host)
        {
            GenericAction attackDiceModification = new FirstOrderVanguardAttackActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableDiceModification(attackDiceModification);

            GenericAction defenceDiceModification = new FirstOrderVanguardDefenceActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip,
                Source = HostUpgrade
            };
            host.AddAvailableDiceModification(defenceDiceModification);
        }
    }
}

namespace ActionsList
{
    public class FirstOrderVanguardAttackActionEffect: GenericAction
    {
        public FirstOrderVanguardAttackActionEffect()
        {
            Name = DiceModificationName = "First Order Vanguard";
            IsReroll = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (IsDefenderOnlyShipInArcAtShotDistance()) result = true;
            }
            return result;
        }

        private bool IsDefenderOnlyShipInArcAtShotDistance()
        {
            bool result = true;

            foreach (var shipHolder in Roster.AllShips)
            {
                if (shipHolder.Value.ShipId != Host.ShipId && shipHolder.Value.ShipId != Combat.Defender.ShipId)
                {
                    ShotInfo shotInfo = new ShotInfo(Host, shipHolder.Value, Host.PrimaryWeapon);
                    if (shotInfo.InArc && shotInfo.Range <= 3)
                    {
                        return false;
                    }
                }
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                if (Combat.Attacker.GetAvailableDiceModifications().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
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
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 1,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }
    }

    public class FirstOrderVanguardDefenceActionEffect : GenericAction
    {
        public FirstOrderVanguardDefenceActionEffect()
        {
            Name = DiceModificationName = "First Order Vanguard";
            IsReroll = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Defence)
            {
                result = true;
            }
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            // TODO: AI (~ as Han Solo)

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                CallBack = delegate { Source.TryDiscard(callBack); }
            };
            diceRerollManager.Start();
            SelectAllRerolableDices();
            diceRerollManager.ConfirmRerollButtonIsPressed();
        }

        private static void SelectAllRerolableDices()
        {
            Combat.CurrentDiceRoll.SelectBySides
            (
                new List<DieSide>()
                {
                    DieSide.Blank,
                    DieSide.Focus,
                    DieSide.Success,
                    DieSide.Crit
                },
                int.MaxValue
            );
        }
    }
}
