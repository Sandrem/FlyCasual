using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEDDefender
{
    public class DarthVader : TIEDDefender
    {
        public DarthVader() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Darth Vader",
                "Dark Lord of the Sith",
                Faction.Imperial,
                6,
                9,
                14,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DarthVaderDefenderAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Tie,
                    Tags.DarkSide,
                    Tags.Sith
                },
                abilityText: "You may not spend force charges except when attacking. While you perform an attack, you may spend 1 force charge to turn a blank result into a hit.",
                force: 3
            );

            PilotNameCanonical = "darthvader-tieddefender";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/23/1c/231cae81-8fda-49a0-964d-9fb544b5e846/swz84_pilot_darthvader.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DarthVaderDefenderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: SpendForce
            );

            HostShip.OnCheckCanUseForceNow += AddRestriction;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack &&
                Combat.DiceRollAttack.Blanks > 0 &&
                HostShip.State.Force > 0;
        }

        private int GetAiPriority()
        {
            return 45;
        }

        private void SpendForce(Action<bool> callback)
        {
            HostShip.State.SpendForce(1, delegate { callback(true); });
        }

        private void AddRestriction(ref bool isAllowed)
        {
            if (Combat.Attacker == null || Combat.Attacker.ShipId != HostShip.ShipId) isAllowed = false;
        }
    }
}