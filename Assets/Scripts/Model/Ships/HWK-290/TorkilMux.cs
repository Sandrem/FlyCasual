using Arcs;
using BoardTools;
using RuleSets;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace HWK290
    {
        public class TorkilMux : HWK290, ISecondEditionPilot
        {
            public TorkilMux() : base()
            {
                PilotName = "Torkil Mux";
                PilotSkill = 2;
                Cost = 36;

                faction = Faction.Scum;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.TorkilMuxAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TorkilMuxAbilitySE : RoarkGarnetAbility
    {
        protected override string GenerateAbilityMessage()
        {
            return "Choose another ship in arc.\nUntil the end of the phase, treat that ship's pilot skill value as \"0\".";
        }

        public override void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 0;
        }

        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterTargetsByRange(ship, 1, 3) && 
                Board.IsShipInArc(HostShip, ship);
        }
    }
}
