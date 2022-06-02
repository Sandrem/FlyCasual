using Upgrade;
using Ship;
using Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using SubPhases;
using UnityEngine;
using Content;

namespace UpgradesList.SecondEdition
{
    public class SabineWren : GenericUpgrade
    {
        public SabineWren() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sabine Wren",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.SabineWrenCrewAbility),
                seImageNumber: 92,
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(474, 13)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SabineWrenCrewAbility : GenericAbility
    {
        Dictionary<GenericToken, string> ReadyTokens = new Dictionary<GenericToken, string>()
        {
            { new IonToken(null), "I" },
            { new JamToken(null, null), "J" },
            { new StressToken(null), "S" },
            { new TractorBeamToken(null, null), "TB" }
        };

        private GenericShip BombEffectTargetShip;

        public override void ActivateAbility()
        {
            GenericShip.OnAfterSufferBombEffect += CheckAbility;
            Phases.Events.OnGameStart += UpdateNameFirstTime;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAfterSufferBombEffect -= CheckAbility;
            Phases.Events.OnGameStart -= UpdateNameFirstTime;
        }

        private void UpdateNameFirstTime()
        {
            Phases.Events.OnGameStart -= UpdateNameFirstTime;
            UpdateName();
        }

        private void UpdateName()
        {
            string postfix = "";
            foreach (var item in ReadyTokens)
            {
                if (ReadyTokens.Last().Key != item.Key)
                {
                    postfix += item.Value + ", ";
                }
                else
                {
                    postfix += item.Value;
                }
            }

            if (postfix == "") postfix = "no tokens";
            postfix = "(" + postfix + ")";

            HostUpgrade.NamePostfix = postfix;
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        private void CheckAbility(GenericShip ship, GenericBomb bomb)
        {
            if (bomb.HostShip.Owner.PlayerNo == HostShip.Owner.PlayerNo && bomb.UpgradeInfo.SubType == UpgradeSubType.Bomb && ReadyTokens.Any())
            {
                BombEffectTargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnAfterSufferBombEffect, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            if (BombEffectTargetShip.IsDestroyed || BombEffectTargetShip.IsReadyToBeDestroyed)
            {
                Triggers.FinishTrigger();
            }
            else
            {
                Selection.ChangeAnotherShip(BombEffectTargetShip);

                SabineWrenDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<SabineWrenDecisionSubphase>(
                    "Sabine Wren: Select token to assign",
                    Triggers.FinishTrigger
                );

                foreach (var item in ReadyTokens)
                {
                    subphase.AddDecision(
                        item.Key.Name,
                        delegate { AssignToken(item.Key); }
                    );
                }

                subphase.AddDecision(
                    "None",
                    delegate { AssignToken(null); }
                );

                subphase.DescriptionShort = "Sabine Wren";
                subphase.DescriptionLong = string.Format("Select token to assign to {0} (ID:{1})", BombEffectTargetShip.PilotInfo.PilotName, BombEffectTargetShip.ShipId);
                subphase.ImageSource = HostUpgrade;

                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
                subphase.RequiredPlayer = HostShip.Owner.PlayerNo;
                subphase.ShowSkipButton = true;
                subphase.Start();
            }
        }

        private void AssignToken(GenericToken token)
        {
            Selection.DeselectAnotherShip();
            DecisionSubPhase.ConfirmDecisionNoCallback();

            if (token == null)
            {
                Triggers.FinishTrigger();
            }
            else if (token is JamToken)
            {
                ReadyTokens.Remove(token);
                UpdateName();

                BombEffectTargetShip.Tokens.AssignToken(
                    new JamToken(BombEffectTargetShip, HostShip.Owner),
                    Triggers.FinishTrigger
                );
            }
            else if (token is TractorBeamToken)
            {
                ReadyTokens.Remove(token);
                UpdateName();

                BombEffectTargetShip.Tokens.AssignToken(
                    new TractorBeamToken(BombEffectTargetShip, HostShip.Owner),
                    Triggers.FinishTrigger
                );
            }
            else
            {
                ReadyTokens.Remove(token);
                UpdateName();

                BombEffectTargetShip.Tokens.AssignToken(
                    token.GetType(),
                    Triggers.FinishTrigger
                );
            }
        }

        private class SabineWrenDecisionSubphase : DecisionSubPhase { };
    }
}