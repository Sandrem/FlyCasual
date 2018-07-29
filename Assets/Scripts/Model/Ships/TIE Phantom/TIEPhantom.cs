using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using RuleSets;
using Ship;
using Tokens;

namespace Ship
{
    namespace TIEPhantom
    {
        public class TIEPhantom : GenericShip, TIE, ISecondEditionShip
        {

            public TIEPhantom() : base()
            {
                Type = FullType = "TIE Phantom";
                IconicPilots.Add(Faction.Imperial, typeof(Whisper));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/c/ce/MI_TIE-PHANTOM.png";

                Firepower = 4;
                Agility = 2;
                MaxHull = 2;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                ActionBar.AddPrintedAction(new EvadeAction());
                ActionBar.AddPrintedAction(new BarrelRollAction());
                ActionBar.AddPrintedAction(new CloakAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEPhantomTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Gray";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 4;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", MovementComplexity.Normal);
                Maneuvers.Add("1.R.B", MovementComplexity.None);
                Maneuvers.Add("1.R.T", MovementComplexity.Normal);
                Maneuvers.Add("1.F.R", MovementComplexity.None);
                Maneuvers.Add("2.L.T", MovementComplexity.Normal);
                Maneuvers.Add("2.L.B", MovementComplexity.Easy);
                Maneuvers.Add("2.F.S", MovementComplexity.Easy);
                Maneuvers.Add("2.R.B", MovementComplexity.Easy);
                Maneuvers.Add("2.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.T", MovementComplexity.Normal);
                Maneuvers.Add("3.L.B", MovementComplexity.Normal);
                Maneuvers.Add("3.F.S", MovementComplexity.Easy);
                Maneuvers.Add("3.R.B", MovementComplexity.Normal);
                Maneuvers.Add("3.R.T", MovementComplexity.Normal);
                Maneuvers.Add("3.F.R", MovementComplexity.Complex);
                Maneuvers.Add("4.F.S", MovementComplexity.Normal);
                Maneuvers.Add("4.F.R", MovementComplexity.Complex);
            }

            public void AdaptShipToSecondEdition()
            {
                Maneuvers.Add("1.L.B", MovementComplexity.Normal);
                Maneuvers.Add("1.F.S", MovementComplexity.Normal);

                FullType = "TIE/ph Phantom";

                MaxHull = 3;

                ShipAbilities.Add(new Abilities.SecondEdition.StygiumArray());

                IconicPilots[Faction.Imperial] = typeof(SigmaSquadronAce);
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StygiumArray : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDecloak += RegisterAssignEvade;
            Phases.Events.OnEndPhaseStart_Triggers += RegisterCloakAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDecloak -= RegisterAssignEvade;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterCloakAbility;
        }

        private void RegisterAssignEvade()
        {
            RegisterAbilityTrigger(TriggerTypes.OnDecloak, AssignEvadeToken);
        }

        private void AssignEvadeToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Stygium Array: Evade token is assigned");

            HostShip.Tokens.AssignToken(typeof(EvadeToken), Triggers.FinishTrigger);
        }

        private void RegisterCloakAbility()
        {
            if (HostShip.Tokens.HasToken<EvadeToken>())
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToCloak);
            }
        }

        private void AskToCloak(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, TradeEvadeForCloakToken, infoText: "Spend Evade Token to gain Cloak Token?");
        }

        private void TradeEvadeForCloakToken(object sender, System.EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            if (HostShip.Tokens.HasToken<EvadeToken>())
            {
                HostShip.Tokens.RemoveToken(typeof(EvadeToken), AssignCloakToken);
            }
            else
            {
                Messages.ShowError("Ship doesn't have Evade token to spend!");
                Triggers.FinishTrigger();
            }
        }

        private void AssignCloakToken()
        {
            HostShip.Tokens.AssignToken(typeof(CloakToken), Triggers.FinishTrigger);
        }
    }
}
