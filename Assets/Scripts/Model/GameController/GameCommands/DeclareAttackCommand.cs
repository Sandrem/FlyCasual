using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameCommands
{
    public class DeclareAttackCommand : GameCommand
    {
        public DeclareAttackCommand(GameCommandTypes type, Type subPhase, string rawParameters) : base(type, subPhase, rawParameters)
        {

        }

        public override void Execute()
        {
            Combat.DeclareIntentToAttack(
                int.Parse(GetString("id")),
                int.Parse(GetString("target")),
                bool.Parse(GetString("weaponIsAlreadySelected"))
            );
        }
    }

}
