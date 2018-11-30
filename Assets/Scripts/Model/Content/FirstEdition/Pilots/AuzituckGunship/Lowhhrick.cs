using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AuzituckGunship
    {
        public class Lowhhrick : AuzituckGunship
        {
            public Lowhhrick() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lowhhrick",
                    5,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.LowhhrickAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Lowhhrick";
            }
        }
    }
}


namespace Abilities.FirstEdition
{
    public class LowhhrickAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddLowhhrickAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddLowhhrickAbility;
        }

        private void AddLowhhrickAbility(GenericShip ship)
        {
            Combat.Defender.AddAvailableDiceModification(new DiceModificationAction() { Host = this.HostShip });
        }

        private class DiceModificationAction : ActionsList.GenericAction
        {
            public DiceModificationAction()
            {
                Name = DiceModificationName = "Lowhhrick's ability";

                TokensSpend.Add(typeof(ReinforceForeToken));
                TokensSpend.Add(typeof(ReinforceAftToken));
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    if (Combat.Defender.ShipId != Host.ShipId)
                    {
                        if (Combat.Defender.Owner.PlayerNo == Host.Owner.PlayerNo)
                        {
                            if (Host.Tokens.HasToken(typeof(ReinforceForeToken)) || Host.Tokens.HasToken(typeof(ReinforceAftToken)))
                            {
                                BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(Host, Combat.Defender);
                                if (positionInfo.Range <= 1)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                // TODORESTORE

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

                GenericToken reinforceToken = (Host.Tokens.HasToken(typeof(ReinforceForeToken))) ? Host.Tokens.GetToken(typeof(ReinforceForeToken)) : Host.Tokens.GetToken(typeof(ReinforceAftToken));
                Host.Tokens.SpendToken(reinforceToken.GetType(), callBack);
            }
        }

    }
}
