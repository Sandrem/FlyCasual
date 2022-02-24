using BoardTools;
using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class MaceWindu : Delta7Aethersprite
    {
        public MaceWindu()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Mace Windu",
                "Harsh Traditionalist",
                Faction.Republic,
                4,
                4,
                7,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.MaceWinduAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Mace Windu"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/de/33/de3326f7-521c-4f50-8599-483db5f32d6d/swz32_mace-windu.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver, recover 1 force.
    public class MaceWinduAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && !(Board.IsOffTheBoard(HostShip) || HostShip.IsBumped))
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignTokens);
            }
        }
        
        private void AssignTokens(object sender, EventArgs e)
        {
            HostShip.State.RestoreForce();
            Triggers.FinishTrigger();
        }
    }
}
