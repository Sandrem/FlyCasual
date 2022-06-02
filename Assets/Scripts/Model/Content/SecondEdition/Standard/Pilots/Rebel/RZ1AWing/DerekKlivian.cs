using Content;
using Mods.ModsList;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.RZ1AWing
{
    public class DerekKlivian : RZ1AWing
    {
        public DerekKlivian() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Derek Klivian",
                "Hobbie",
                Faction.Rebel,
                3,
                3,
                6,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DerekKlivianAbility),
                extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Blue"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d3/b1/d3b16ec1-de8e-446c-8fab-b7749a6537c8/swz83_pilot_derekklivian.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DerekKlivianAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += CheckDerekKlivialAbility;
            HostShip.OnTokenIsAssigned += CheckDerekKlivialAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= CheckDerekKlivialAbility;
            HostShip.OnTokenIsAssigned -= CheckDerekKlivialAbility;
        }

        private void CheckDerekKlivialAbility(GenericShip ship, GenericToken token)
        {
            if (token is BlueTargetLockToken)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, TryToRemoveRedToken);
            }
        }

        private void TryToRemoveRedToken(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasTokenByColor(TokenColors.Red))
            {
                AskToRemoveRedToken();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AskToRemoveRedToken()
        {
            DerekKlivianSubPhase subPhase = Phases.StartTemporarySubPhaseNew<DerekKlivianSubPhase>("Derek Klivian Subphase", Triggers.FinishTrigger);

            subPhase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subPhase.DescriptionLong = "You may remove 1 red token from yourself";
            subPhase.ImageSource = HostShip;

            foreach (GenericToken token in HostShip.Tokens.GetTokensByColor(TokenColors.Red))
            {
                if (!subPhase.GetDecisions().Any(n => n.Name == token.Name))
                {
                    subPhase.AddDecision(token.Name, delegate { RemoveTokenByType(token.GetType()); });
                }
            }

            subPhase.DecisionOwner = HostShip.Owner;
            subPhase.DefaultDecisionName = subPhase.GetDecisions().First().Name;
            subPhase.ShowSkipButton = true;

            subPhase.Start();
        }

        private void RemoveTokenByType(Type type)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.RemoveToken(type, Triggers.FinishTrigger);
        }

        private class DerekKlivianSubPhase : DecisionSubPhase { }
    }
}