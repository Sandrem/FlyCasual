﻿using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using Actions;
using Tokens;
using System;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class C3PORepublic : GenericUpgrade
    {
        public C3PORepublic() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "C-3PO",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(CalculateAction)),
                abilityType: typeof(Abilities.SecondEdition.C3PORepublicCrewAbility)
            );

            NameCanonical = "c3po-republic";

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(218, 0)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/8b/b8/8bb8be49-b567-4a31-a17e-b8ca2b86b039/swz48_cards-c-3p0.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class C3PORepublicCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;

            AddDiceModification(
                "C-3PO",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        private int GetAiPriority()
        {
            // Activate it only to reroll blanks
            return (Combat.DiceRollDefence.Blanks > 0) ? 90 : 0;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence
                && HostShip.Tokens.HasToken<CalculateToken>();                
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;

            RemoveDiceModification();
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is CalculateAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignCalculateToken);
            }
        }

        private void AssignCalculateToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("C-3PO: Additional calculate token is gained");
            HostShip.Tokens.AssignToken(typeof(CalculateToken), Triggers.FinishTrigger);
        }

    }
}