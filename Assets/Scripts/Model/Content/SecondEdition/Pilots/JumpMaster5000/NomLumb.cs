using Arcs;
using BoardTools;
using Ship;
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
                PilotInfo = new PilotCardInfo(
                    "Nom Lumb",
                    1,
                    38,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NomLumbAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/4a13a4b7493d39f53b9c37c6a82edf5a.png";
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

        // TODO: Allow to select arc if in 2

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

                List<ArcType> standatdArcs = new List<ArcType>() { ArcType.Front, ArcType.Left, ArcType.Right, ArcType.Rear };
                foreach (ArcType facing in standatdArcs)
                {
                    if (HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, facing))
                    {
                        ArcFacing direction = ArcFacing.Front;
                        switch (facing)
                        {
                            case ArcType.Front:
                                direction = ArcFacing.Front;
                                break;
                            case ArcType.Rear:
                                direction = ArcFacing.Rear;
                                break;
                            case ArcType.Left:
                                direction = ArcFacing.Left;
                                break;
                            case ArcType.Right:
                                direction = ArcFacing.Right;
                                break;
                            default:
                                break;
                        }

                        HostShip.ArcsInfo.GetArc<ArcSingleTurret>().RotateArc(direction);
                    }
                }
            }
        }
    }
}