using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class HeraSyndulla : ASF01BWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    6,
                    55,
                    pilotTitle: "Phoenix Leader",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HeraSyndullaBWingAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "herasyndulla-asf01bwing";

                ModelInfo.SkinName = "Prototype";

                ImageUrl = "https://i.imgur.com/eGmyOQm.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HeraSyndullaBWingAbility : GenericAbility
    {
        private GenericShip ShipToTransferToken;

        private static readonly List<Type> TokenTypesTransferable = new List<Type>()
        {
            typeof(Tokens.FocusToken),
            typeof(Tokens.EvadeToken),
            typeof(Tokens.BlueTargetLockToken)
        };

        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (!HasTokensToTransfter()) return;

            if (Tools.IsAnotherFriendly(HostShip, Combat.Attacker))
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Attacker);
                if (distInfo.Range >= 1 && distInfo.Range <= 2)
                {
                    ShipToTransferToken = Combat.Attacker;
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToTransfterToken);
                }
            }
            else if(Tools.IsAnotherFriendly(HostShip, Combat.Defender))
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Defender);
                if (distInfo.Range >= 1 && distInfo.Range <= 2)
                {
                    ShipToTransferToken = Combat.Defender;
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToTransfterToken);
                }
            }
        }

        private bool HasTokensToTransfter()
        {
            foreach (Type tokenType in TokenTypesTransferable)
            {
                if (HostShip.Tokens.HasToken(tokenType, '*')) return true;
            }

            return false;
        }

        private void AskToTransfterToken(object sender, EventArgs e)
        {
            HeraSyndullaDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<HeraSyndullaDecisionSubPhase>("Hera Syndulla Decision", Triggers.FinishTrigger);

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = $"Do you want to transfer 1 of your tokens to {ShipToTransferToken.PilotInfo.PilotName}?";
            subphase.ImageSource = HostShip;

            foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
            {
                if (TokenTypesTransferable.Contains(token.GetType()))
                {
                    string tokenName = (token is BlueTargetLockToken) ? $"Lock \"{(token as BlueTargetLockToken).Letter}\"" : token.Name;
                    subphase.AddDecision(tokenName, delegate { TransferToken(token); });
                }
            }

            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            subphase.DefaultDecisionName = "No";
            
            subphase.DecisionOwner = HostShip.Owner;

            subphase.Start();
        }

        private void TransferToken(GenericToken token)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            ActionsHolder.ReassignToken(token, HostShip, ShipToTransferToken, Triggers.FinishTrigger);
        }

        private class HeraSyndullaDecisionSubPhase : DecisionSubPhase { };
    }
}