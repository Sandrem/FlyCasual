using Arcs;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class NomLumb : JumpMaster5000
        {
            public NomLumb() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Nom Lumb",
                    "On the Run",
                    Faction.Scum,
                    1,
                    5,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NomLumbAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/4a13a4b7493d39f53b9c37c6a82edf5a.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NomLumbAbility : GenericAbility
    {
        // After you become the defender, if the attacker is not in your single turret arc, you must rotate your
        // single turret arc indicator to a standard arc the attacker is in.

        List<ArcFacing> FacingsToAttacker = new List<ArcFacing>();

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsDefender += CheckArcRotation;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsDefender -= CheckArcRotation;
        }

        private void CheckArcRotation()
        {
            ShotInfo reverseShotInfo = new ShotInfo(HostShip, Combat.Attacker, HostShip.PrimaryWeapons.First());
            if (!reverseShotInfo.InArcByType(ArcType.SingleTurret))
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Attacker is not in turret arc");
                
                FacingsToAttacker = GetFacingsToAttacker();
                if (FacingsToAttacker.Count == 1)
                {
                    HostShip.ArcsInfo.GetArc<ArcSingleTurret>().RotateArc(FacingsToAttacker.First());
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Arc indicator is rotated");
                }
                else if (FacingsToAttacker.Count > 1)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskWhatArcToUse);
                }
                else
                {
                    Messages.ShowError(HostShip.PilotInfo.PilotName + ": Cannot find arc with attacker");
                }
            }
        }

        private void AskWhatArcToUse(object sender, EventArgs e)
        {
            NomLumbArcRotationSubphase subphase = Phases.StartTemporarySubPhaseNew<NomLumbArcRotationSubphase>(
                HostShip.PilotInfo.PilotName + ": Arc Rotation Decision",
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "You must rotate arc indicator";
            subphase.ImageSource = HostShip;

            foreach (ArcFacing facing in FacingsToAttacker)
            {
                subphase.AddDecision(
                    "Rotate to " + facing,
                    delegate {
                        HostShip.ArcsInfo.GetArc<ArcSingleTurret>().RotateArc(facing);
                        DecisionSubPhase.ConfirmDecision();
                    }
                );
            }

            subphase.DecisionOwner = HostShip.Owner;

            //TODO: Improve: count how many ships in each arc
            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private List<ArcFacing> GetFacingsToAttacker()
        {
            List<ArcFacing> result = new List<ArcFacing>();

            List<ArcType> standatdArcs = new List<ArcType>() { ArcType.Front, ArcType.Left, ArcType.Right, ArcType.Rear };
            foreach (ArcType arcType in standatdArcs)
            {
                if (HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, arcType))
                {
                    result.Add(ArcTypeToFacing(arcType));
                }
            }

            return result;
        }

        private ArcFacing ArcTypeToFacing(ArcType type)
        {
            ArcFacing facing = ArcFacing.None;

            switch (type)
            {
                case ArcType.Front:
                    facing = ArcFacing.Front;
                    break;
                case ArcType.Rear:
                    facing = ArcFacing.Rear;
                    break;
                case ArcType.Left:
                    facing = ArcFacing.Left;
                    break;
                case ArcType.Right:
                    facing = ArcFacing.Right;
                    break;
                default:
                    break;
            }

            return facing;
        }

        public class NomLumbArcRotationSubphase : DecisionSubPhase { };
    }
}