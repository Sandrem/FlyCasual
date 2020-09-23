using System;
using System.Collections.Generic;
using BoardTools;
using Ship;

namespace Ship
{
    namespace SecondEdition.XiClassLightShuttle
    {
        public class CommanderMalarus : XiClassLightShuttle
        {
            public CommanderMalarus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Commander Malarus",
                    5,
                    43,
                    pilotTitle: "Vindictive Taskmaster",
                    isLimited: true,
                    extraUpgradeIcon: Upgrade.UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.CommanderMalarusXiClassLightShuttleAbility)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/85/c4/85c41c14-3071-401b-b3ee-89d048acd9f0/swz69_a1_ship_malarus.png";

                PilotNameCanonical = "commandermalarus-xiclasslightshuttle";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship at range 0-2 performs a primary attack, if it has 1 or more blank results,
    //that ship must gain 1 strain token to reroll 1 blank result, if able
    public class CommanderMalarusXiClassLightShuttleAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1,
                sidesCanBeSelected: new List<DieSide> { DieSide.Blank },
                isGlobal: true,
                isForcedModification: true,
                payAbilityCost: AssignStrainToken
            );
        }

        private void AssignStrainToken(Action<bool> callback)
        {
            Combat.Attacker.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                delegate { callback(true); }
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack &&
                Combat.Attacker.Owner == HostShip.Owner &&
                Combat.ChosenWeapon is PrimaryWeaponClass &&
                Combat.DiceRollAttack.Blanks > 0 &&
                new DistanceInfo(Combat.Attacker, HostShip).Range <= 2;
        }

        private int GetAiPriority()
        {
            return int.MaxValue;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}

