using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEWiWhisperModifiedInterceptor
    {
        public class Whirlwind : TIEWiWhisperModifiedInterceptor
        {
            public Whirlwind() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Whirlwind\"",
                    "Reap What You Sow",
                    Faction.FirstOrder,
                    3,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WhirlwindPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/vaWIuzX.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WhirlwindPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += DoRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= DoRegisterAbility;
        }

        private void DoRegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, PerformAbility);
        }

        private void PerformAbility(object sender, EventArgs e)
        {
            AskToRemoveJamTokens(GainFocusTokens);
        }

        private void AskToRemoveJamTokens(Action callback)
        {
            int jamTokensCount = HostShip.Tokens.CountTokensByType<JamToken>();

            if (jamTokensCount == 0)
            {
                callback();
            }
            else
            {
                WhirwindSpecialDecisonSubphase subphase = Phases.StartTemporarySubPhaseNew<WhirwindSpecialDecisonSubphase>(
                    "Whirlwind jam decision",
                    callback
                );

                subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
                subphase.DescriptionLong = "You may remove any number of jam tokens";
                subphase.ImageSource = HostShip;

                subphase.AddDecision("Remove all", delegate { DoRemoveJamTokens(jamTokensCount, callback); });
                subphase.AddDecision("Leave 1", delegate { DoRemoveJamTokens(jamTokensCount-1, callback); });

                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
                subphase.DecisionOwner = HostShip.Owner;

                subphase.Start();
            }
        }

        private void DoRemoveJamTokens(int count, Action callback)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {count} Jam token(s) are removed");

            List<GenericToken> tokensToRemove = HostShip.Tokens.GetAllTokens().Where( n => n is JamToken).Take(count).ToList();
            HostShip.Tokens.RemoveTokens(tokensToRemove, callback);
        }

        private void GainFocusTokens()
        {
            int countEnemyShipsTargeting = 0;

            foreach (GenericShip ship in HostShip.Owner.EnemyShips.Values)
            {
                if (ship.SectorsInfo.IsShipInSector(HostShip, Arcs.ArcType.Front)) countEnemyShipsTargeting++;
            }

            if (countEnemyShipsTargeting == 0)
            {
                Triggers.FinishTrigger();
            }
            else
            {
                AskToGetFocusTokens(countEnemyShipsTargeting);
            }
        }

        private void AskToGetFocusTokens(int count)
        {
            if (alwaysUseAbility)
            {
                DoGetFocusTokens(count);
            }
            else
            {
                AskToUseAbility
                (
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    delegate
                    {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        DoGetFocusTokens(count);
                    },
                    showAlwaysUseOption: true,
                    descriptionLong: $"Do you want to get {count} Focus Token(s)?",
                    imageHolder: HostShip,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
        }

        private void DoGetFocusTokens(int count)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} gains {count} Focus Token(s)");
            HostShip.Tokens.AssignTokens(CreateFocusToken, count, Triggers.FinishTrigger);
        }

        private GenericToken CreateFocusToken()
        {
            return new FocusToken(HostShip);
        }

        private class WhirwindSpecialDecisonSubphase : DecisionSubPhase { }
    }
}
