using Actions;
using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class Guri : StarViperClassAttackPlatform
        {
            public Guri() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Guri",
                    "Prince Xizor’s Bodyguard",
                    Faction.Scum,
                    5,
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GuriAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 178,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GuriAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterGuriAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterGuriAbility;
        }

        private void RegisterGuriAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskGuriAbility);
        }
        private void AskGuriAbility(object sender, EventArgs e)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(0, 1), Team.Type.Enemy).Count > 0)
            {
                if (!alwaysUseAbility)
                {
                    AskToUseAbility(
                        HostShip.PilotInfo.PilotName,
                        AlwaysUseByDefault,
                        UseAbility,
                        descriptionLong: "Do you want to gain 1 Focus Token?",
                        imageHolder: HostShip,
                        showAlwaysUseOption: true
                    );
                }
                else
                {
                    AssignFocus(Triggers.FinishTrigger);
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            AssignFocus(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void AssignFocus(Action callback)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), callback);
        }

    }
}