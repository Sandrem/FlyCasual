using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Tokens;
using Ship.CardInfo;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.TIEPhPhantom
    {
        public class TIEPhPhantom : GenericShip
        {
            public TIEPhPhantom() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "TIE/ph Phantom",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Imperial, typeof(Echo) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 2, 2, 2,
                    new ShipActionsInfo(
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(EvadeAction)),
                        new ActionInfo(typeof(BarrelRollAction)),
                        new ActionInfo(typeof(CloakAction))
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "TIE Phantom",
                    "Gray",
                    new Vector3(-3.28f, 7.4f, 5.55f),
                    1.5f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "TIE-Fly1",
                        "TIE-Fly2",
                        "TIE-Fly3",
                        "TIE-Fly4",
                        "TIE-Fly5",
                        "TIE-Fly6",
                        "TIE-Fly7"
                    },
                    "TIE-Fire", 4
                );

                ShipIconLetter = 'P';
                
                ShipAbilities.Add(new Abilities.SecondEdition.StygiumArray());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StygiumArray : GenericAbility
    {
        public override string Name { get { return "Stygium Array (ID:" + HostShip.ShipId + ")"; } }

        public override void ActivateAbility()
        {
            HostShip.OnDecloak += RegisterPerformFreeEvadeAction;
            Phases.Events.OnEndPhaseStart_Triggers += RegisterCloakAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDecloak -= RegisterPerformFreeEvadeAction;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterCloakAbility;
        }

        private void RegisterPerformFreeEvadeAction()
        {
            RegisterAbilityTrigger(TriggerTypes.OnDecloak, ProposeFreeEvadeAction);
        }

        private void ProposeFreeEvadeAction(object sender, System.EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new EvadeAction() { HostShip = HostShip },
                Triggers.FinishTrigger,
                descriptionShort: Name,
                descriptionLong: "After you decloak, you may perform an Evade action"
            );
        }

        private void RegisterCloakAbility()
        {
            if (HostShip.Tokens.HasToken<EvadeToken>() && !(HostShip.Tokens.HasToken<CloakToken>()))
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToCloak);
            }
        }

        private void AskToCloak(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                "Stygium Array",
                NeverUseByDefault,
                TradeEvadeForCloakToken,
                descriptionLong: "Do you want to spend an Evade Token to gain a Cloak Token?"
            );
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
                Messages.ShowError(HostShip.PilotInfo.PilotName + " doesn't have any Evade token to spend!");
                Triggers.FinishTrigger();
            }
        }

        private void AssignCloakToken()
        {
            HostShip.Tokens.AssignToken(typeof(CloakToken), Triggers.FinishTrigger);
        }
    }
}
