using BoardTools;
using Content;
using Players;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class PalobGodalhi : Hwk290LightFreighter
        {
            public PalobGodalhi() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Palob Godalhi",
                    "Tethan Resister",
                    Faction.Scum,
                    3,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.PalobGodalhiAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 175
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PalobGodalhiAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        private void Ability(object sender, EventArgs e)
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                Selection.ChangeActiveShip(HostShip);
                Messages.ShowInfoToHuman(HostShip.PilotInfo.PilotName + ": Select a ship to remove Focus/Evade token from");

                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Choose a ship to remove 1 focus or evade token from it and assign this token to yourself",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) &&
                FilterTargetsByRange(ship, 0, 2) &&
                Board.IsShipInArc(HostShip, ship) &&
                FilterTargetWithTokens(ship);
        }

        private bool FilterTargetWithTokens(GenericShip ship)
        {
            return (ship.Tokens.HasToken(typeof(FocusToken)) || ship.Tokens.HasToken(typeof(EvadeToken)));
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(FocusToken));
            int shipEvadeTokens = ship.Tokens.CountTokensByType(typeof(EvadeToken));

            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);
            if (shipFocusTokens > 0)
                result += 50;
            if (shipFocusTokens == 1)
                result += 100;
            if (shipEvadeTokens > 0)
                result += 25;

            return result;
        }

        private void SelectAbilityTarget()
        {
            GenericShip thisship = TargetShip;
            int numfocustokens = thisship.Tokens.CountTokensByType(typeof(FocusToken));
            int numevadetokens = thisship.Tokens.CountTokensByType(typeof(EvadeToken));

            if (numfocustokens > 0 && numevadetokens == 0)
            {
                TakeFocus();
            }
            else
            {
                if (numfocustokens == 0 && numevadetokens > 0)
                {
                    takeEvade();
                }
                else
                {
                    if (numfocustokens > 0 && numevadetokens > 0)
                    {
                        AskWhichTokenToTake(takeFocusEventHandler, takeEvadeEventHandler);
                    }
                    else
                    {
                        SelectShipSubPhase.FinishSelection();
                    }
                }
            }
        }

        private void TakeFocus()
        {
            TargetShip.Tokens.RemoveToken(
                typeof(FocusToken),
                delegate {
                    HostShip.Tokens.AssignToken(
                        typeof(FocusToken),
                        delegate {
                            SelectShipSubPhase.FinishSelection();
                        }
                    );
                }
            );
        }

        private void takeEvade()
        {
            TargetShip.Tokens.RemoveToken(
                typeof(EvadeToken),
                delegate {
                    HostShip.Tokens.AssignToken(
                        typeof(EvadeToken),
                        delegate {
                            SelectShipSubPhase.FinishSelection();
                        }
                    );
                }
            );
        }

        private void takeFocusEventHandler(object sender, EventArgs e)
        {
            TargetShip.Tokens.RemoveToken(
                typeof(FocusToken),
                delegate {
                    HostShip.Tokens.AssignToken(
                        typeof(FocusToken),
                        delegate {
                            WhichTokenDecisionSubphase.ConfirmDecisionNoCallback();
                            SelectShipSubPhase.FinishSelection();
                        }
                    );
                }
            );
        }

        private void takeEvadeEventHandler(object sender, EventArgs e)
        {
            TargetShip.Tokens.RemoveToken(
                typeof(EvadeToken),
                delegate {
                    HostShip.Tokens.AssignToken(
                        typeof(EvadeToken),
                        delegate {
                            WhichTokenDecisionSubphase.ConfirmDecisionNoCallback();
                            SelectShipSubPhase.FinishSelection();
                        }
                    );
                }
            );
        }

        private void AskWhichTokenToTake(EventHandler takeFocusHandler, EventHandler takeEvadeHandler, Action callback = null)
        {
            if (callback == null)
                callback = Triggers.FinishTrigger;

            if (HostShip.Owner.PlayerType == PlayerType.Ai)
            {
                TakeFocus();
            }
            else
            {
                DecisionSubPhase whichToken = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(WhichTokenDecisionSubphase),
                    callback
                );

                whichToken.DescriptionShort = HostShip.PilotInfo.PilotName;
                whichToken.DescriptionLong = "Take which type of Token?";
                whichToken.ImageSource = HostShip;

                whichToken.RequiredPlayer = HostShip.Owner.PlayerNo;

                whichToken.AddDecision("Focus", takeFocusHandler);
                whichToken.AddDecision("Evade", takeEvadeHandler);

                whichToken.ShowSkipButton = false;

                whichToken.Start();
            }
        }

        private class WhichTokenDecisionSubphase : DecisionSubPhase { }
    }
}