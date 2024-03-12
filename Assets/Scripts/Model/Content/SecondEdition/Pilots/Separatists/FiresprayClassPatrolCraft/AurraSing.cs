using BoardTools;
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
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class AurraSing : FiresprayClassPatrolCraft
        {
            public AurraSing() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Aurra Sing",
                    "Bane of the Jedi",
                    Faction.Separatists,
                    4,
                    7,
                    10,
                    isLimited: true,
                    force: 1,
                    abilityType: typeof(Abilities.SecondEdition.AurraSingAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.DarkSide,
                        Tags.BountyHunter
                    },
                    skinName: "Jango Fett"
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/da4433d1-3b24-4bcc-a335-d6d5810b596d/SWZ97_AurraSinglegal.png?format=1000w";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AurraSingAbility : GenericAbility
    {
        public List<GenericShip> SelectedShips = new List<GenericShip>();

        public List<GenericToken> ShipOneTokens = new List<GenericToken>();

        public List<GenericToken> ShipTwoTokens = new List<GenericToken>();

        public int ShipIndex = 0;

        public int TokenIndex = 0;

        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= TryRegisterAbility;
        }

        private void TryRegisterAbility(GenericShip ship)
        {
            if (HasEnoughTargets() && HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskToUseOwnAbility);
            }
        }

        private bool HasEnoughTargets()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Enemy)
                .Count() >= 2;
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                StartMultiselect,
                descriptionLong: "Do you want to spend 1 force to choose 2 enemy ships at range 0-1, and transfer any number of orange and red tokens beween them?",
                imageHolder: HostShip
            );
        }

        private void StartMultiselect(object sender, EventArgs e)
        {
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecisionNoCallback);
            MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("Aurra Sing", Phases.CurrentSubPhase.CallBack);

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Filter = FilterMultiSelection;
            subphase.GetAiPriority = GetAiPriority;
            subphase.MaxToSelect = 2;
            subphase.WhenDone = SetupTokenDecisions;

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName; ;
            subphase.DescriptionLong = "Choose 2 enemy ships at range 0-1";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }

        private void SetupTokenDecisions(Action callback)
        {
            if (Selection.MultiSelectedShips.Count < 2)
            {
                callback();
            }
            else
            {
                SelectedShips.AddRange(Selection.MultiSelectedShips);
                ShipOneTokens.AddRange(SelectedShips[0].Tokens.GetTokensByColor(TokenColors.Red, TokenColors.Orange));
                ShipTwoTokens.AddRange(SelectedShips[1].Tokens.GetTokensByColor(TokenColors.Red, TokenColors.Orange));
                AskTransferToken(ShipIndex, TokenIndex, callback);
            }
        }

        private void AskTransferToken(int shipIndex, int tokenIndex, Action callback)
        {
            GenericShip ship = SelectedShips[shipIndex];
            GenericShip otherShip = shipIndex == 0 ? SelectedShips[1] : SelectedShips[0];

            GenericToken token;
            if (shipIndex == 0)
            {
                token = ShipOneTokens[tokenIndex];
            }
            else
            {
                token = ShipTwoTokens[tokenIndex];
            }

            AuraSingDecisonSubphase subphase = Phases.StartTemporarySubPhaseNew<AuraSingDecisonSubphase>(
                "Aura Sing token decision",
                Phases.CurrentSubPhase.CallBack
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "You may transfer this token from " + ship.PilotInfo.PilotName + " to " + otherShip.PilotInfo.PilotName;
            subphase.ImageSource = HostShip;

            string tokenName = (token is RedTargetLockToken) ? $"Lock \"{(token as RedTargetLockToken).Letter}\"" : token.Name;
            subphase.AddDecision("Transfer: " + tokenName, delegate { TransferTokens(true, callback); });
            subphase.AddDecision("Do Not Transfer: " + tokenName, delegate { TransferTokens(false, callback); });
            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.DecisionOwner = HostShip.Owner;

            subphase.Start();
        }

        private void TransferTokens(bool transfer, Action callback)
        {
            DecisionSubPhase.ConfirmDecision();
            bool callAgain = true;

            GenericShip ship = SelectedShips[ShipIndex];
            GenericShip otherShip = ShipIndex == 0 ? SelectedShips[1] : SelectedShips[0];
            GenericToken token;

            if (ShipIndex == 0)
            {
                token = ShipOneTokens[TokenIndex];
                if (TokenIndex == ShipOneTokens.Count - 1)
                {
                    ShipIndex++;
                    TokenIndex = 0;
                }
                else
                {
                    TokenIndex++;
                }
            }
            else
            {
                token = ShipTwoTokens[TokenIndex];
                if (TokenIndex == ShipTwoTokens.Count - 1)
                {
                    callAgain = false;
                }
                else
                {
                    TokenIndex++;
                }
            }

            if (transfer)
            {
                ActionsHolder.ReassignToken(token, ship, otherShip, delegate { });
                Messages.ShowInfo("Token: " + token.Name + " transfered from " + ship.PilotInfo.PilotName + " to " + otherShip.PilotInfo.PilotName);
            }

            if (callAgain)
            {
                AskTransferToken(ShipIndex, TokenIndex, callback);
            }
            else
            {
                callback();
            }
        }

        private bool FilterMultiSelection(GenericShip ship)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return distInfo.Range >= 0 && distInfo.Range <= 1 && ship.Owner != HostShip.Owner;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        private class AuraSingDecisonSubphase : DecisionSubPhase { }
    }
}