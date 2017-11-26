using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace AuzituckGunship
    {
        public class Lowhhrick : AuzituckGunship
        {
            public Lowhhrick() : base()
            {
                PilotName = "Lowhhrick";
                PilotSkill = 5;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Lowhhrick";

                PilotAbilities.Add(new PilotAbilitiesNamespace.LowhhrickAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class LowhhrickAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            GenericShip.AfterGenerateAvailableActionEffectsListGlobal += AddLowhhrickAbility;
            Host.OnDestroyed += RemoveLowhhrickAbility;
        }

        private void AddLowhhrickAbility()
        {
            Combat.Defender.AddAvailableActionEffect(new DiceModificationAction() { Host = this.Host });
        }

        private void RemoveLowhhrickAbility(GenericShip ship)
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal -= AddLowhhrickAbility;
            Host.OnDestroyed -= RemoveLowhhrickAbility;
        }

        private class DiceModificationAction : ActionsList.GenericAction
        {
            public DiceModificationAction()
            {
                Name = EffectName = "Lowhhrick's ability";
            }

            public override bool IsActionEffectAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    if (Combat.Defender.ShipId != Host.ShipId)
                    {
                        if (Host.HasToken(typeof(ReinforceForeToken)) || Host.HasToken(typeof(ReinforceAftToken)))
                        {
                            Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(Host, Combat.Defender);
                            if (positionInfo.Range == 1)
                            {
                                result = true;
                            }
                        }
                    }
                }

                return result;
            }

            public override int GetActionEffectPriority()
            {
                int result = 0;

                /*if (Combat.AttackStep == CombatStep.Attack)
                {
                    int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                    int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                    //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                    if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                    {
                        if (attackBlanks > 0) result = 90;
                    }
                    else
                    {
                        if (attackBlanks + attackFocuses > 0) result = 90;
                    }
                }*/

                return result;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ApplyEvade();

                GenericToken reinforceToken = (Host.HasToken(typeof(ReinforceForeToken))) ? Host.GetToken(typeof(ReinforceForeToken)) : Host.GetToken(typeof(ReinforceAftToken));
                Host.SpendToken(reinforceToken.GetType(), callBack);
            }
        }

    }
}
