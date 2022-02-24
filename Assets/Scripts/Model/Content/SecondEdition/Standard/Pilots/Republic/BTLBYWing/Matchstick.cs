using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class Matchstick : BTLBYWing
        {
            public Matchstick() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Matchstick\"",
                    "Shadow Two",
                    Faction.Republic,
                    4,
                    5,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MatchstickAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone,
                        Tags.YWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b2/2d/b22dd819-8ac1-4767-b870-0212bb2601fa/swz48_pilot-matchstick.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MatchstickAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                GetCount
            );
        }

        private int GetCount()
        {
            return HostShip.Tokens.GetAllTokens().Count(n => n.TokenColor == TokenColors.Red);
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon || Combat.ArcForShot.ArcType == Arcs.ArcType.SingleTurret)
                && HostShip.Tokens.GetAllTokens().Count(n => n.TokenColor == TokenColors.Red) > 0;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
