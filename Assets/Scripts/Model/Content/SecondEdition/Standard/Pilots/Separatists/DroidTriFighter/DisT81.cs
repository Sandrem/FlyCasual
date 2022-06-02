using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class DisT81 : DroidTriFighter
    {
        public DisT81()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DIS-T81",
                "Clever Circuits",
                Faction.Separatists,
                4,
                4,
                12,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DisT81Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Cannon,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/c2/DIS-T81.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DisT81Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "DIS-T81",
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> { DieSide.Focus },
                DieSide.Success,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            if (Combat.CurrentDiceRoll.Focuses == 0) return false;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                foreach (GenericShip ship in HostShip.Owner.Ships.Values)
                {
                    ShotInfo shotInfo = new ShotInfo(Combat.Defender, ship, Combat.Defender.PrimaryWeapons);
                    if (shotInfo.InArc && ship.Tokens.HasToken<CalculateToken>()) return true;
                }
            }
            else if (Combat.AttackStep == CombatStep.Defence)
            {
                foreach (GenericShip ship in HostShip.Owner.Ships.Values)
                {
                    ShotInfo shotInfo = new ShotInfo(Combat.Attacker, ship, Combat.Attacker.PrimaryWeapons);
                    if (shotInfo.InArc && ship.Tokens.HasToken<CalculateToken>()) return true;
                }
            }
            return false;
        }

        private int GetDiceModificationAiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.CurrentDiceRoll.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    int defenceFocuses = Combat.CurrentDiceRoll.Focuses;
                    int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));
                    if (numFocusTokens > 0 && defenceFocuses > 1)
                    {
                        // Multiple focus results on our defense roll and we have a Focus token.  Use it instead of the Calculate.
                        result = 0;
                    }
                    else if (defenceFocuses > 0)
                    {
                        // We don't have a focus token.  Better use the Calculate.
                        result = 41;
                    }
                }

            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.CurrentDiceRoll.Focuses;
                if (attackFocuses > 0)
                {
                    result = 41;
                }
            }

            return result;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            SelectTargetForAbility(
                () => SpendToken(callback),
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                "DIS-T81",
                "Choose a friendly ship in enemy ship's arc to spend a calculate token from it",
                HostUpgrade,
                callback: () => callback(false)
            );
        }

        private void SpendToken(Action<bool> callback)
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            if (TargetShip.Tokens.HasToken<CalculateToken>())
            {
                TargetShip.Tokens.SpendToken(typeof(CalculateToken), () => callback(true));
            }
            else
            {
                callback(false);
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            ShotInfo shotInfo = null;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                shotInfo = new ShotInfo(Combat.Defender, ship, Combat.Defender.PrimaryWeapons);
            }
            else if (Combat.AttackStep == CombatStep.Defence)
            {
                shotInfo = new ShotInfo(Combat.Attacker, ship, Combat.Attacker.PrimaryWeapons);
            }
            return (shotInfo.InArc && ship.Tokens.HasToken<CalculateToken>());
        }

        private int GetAiPriority(GenericShip ship)
        {
            //prioritize cheap ships and ships with multiple calculate tokens
            //a more advanced function could take into account which ships have already attacked, and which are likely to be attacked

            int result = 0;

            result += ship.Tokens.CountTokensByType<CalculateToken>() * 100;

            result += 200 - ship.PilotInfo.Cost;

            return result;
        }
    }
}
