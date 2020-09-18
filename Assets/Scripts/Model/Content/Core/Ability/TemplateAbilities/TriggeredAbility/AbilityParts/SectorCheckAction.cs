using Arcs;
using Ship;
using System;
using Tokens;

namespace Abilities
{
    public class SectorCheckAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public ArcType SectorType { get; }
        public Func<GenericShip> GetTargetShip { get; }
        public AbilityPart Action { get; }

        public SectorCheckAction(ArcType sectorType, Func<GenericShip> targetShip, AbilityPart action)
        {
            SectorType = sectorType;
            GetTargetShip = targetShip;
            Action = action;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            if (Ability.HostShip.SectorsInfo.IsShipInSector(GetTargetShip(), SectorType))
            {
                Action.DoAction(Ability);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
