using BoardTools;
using Content;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BobaFettSep : FiresprayClassPatrolCraft
        {
            public BobaFettSep() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Boba Fett",
                    "Survivor",
                    Faction.Separatists,
                    3,
                    7,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BobaFettSeparatistAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    skinName: "Jango Fett"
                );

                PilotNameCanonical = "bobafett-separatists";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/17/5d/175d51c6-6a7a-4f59-b8c1-44417a746187/swz82_a1_boba-fett.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BobaFettSeparatistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                "Boba Fett",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus                
            );
        }

        private bool IsAvailable()
        {
            int friendlyShipsInRange = Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly).Count();

            return Combat.AttackStep == CombatStep.Defence
                && Combat.DiceRollDefence.HasResult(DieSide.Blank)
                && friendlyShipsInRange == 1;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}