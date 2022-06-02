using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class Dfs081 : VultureClassDroidFighter
    {
        public Dfs081()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DFS-081",
                "Preservation Programming",
                Faction.Separatists,
                3,
                2,
                7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.Dfs081Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b4/04/b4044057-fae9-4638-b758-14339c1ce98a/swz29_dfs-081.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //When a friendly ship at range 0-1 defends, it may spend 1 calculate token to change all crit results to hit results.
    public class Dfs081Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                0,
                new List<DieSide> { DieSide.Crit }, 
                DieSide.Success,
                DiceModificationTimingType.Opposite,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Defender.Owner == HostShip.Owner
                && Combat.Defender.Tokens.HasToken<Tokens.CalculateToken>()
                && new BoardTools.DistanceInfo(HostShip, Combat.Defender).Range <= 1);
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (Combat.Defender.Tokens.HasToken<Tokens.CalculateToken>())
            {
                Combat.Defender.Tokens.SpendToken(typeof(Tokens.CalculateToken), () => callback(true));
            }
            else
            {
                callback(false);
            }
        }

        public int GetDiceModificationAiPriority()
        {
            return 95;
        }
    }
}
