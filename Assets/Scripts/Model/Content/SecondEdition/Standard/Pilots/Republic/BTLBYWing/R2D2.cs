using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class R2D2 : BTLBYWing
        {
            public R2D2() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "R2-D2",
                    "Bucket of Bolts",
                    Faction.Republic,
                    2,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.R2D2PilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid,
                        Tags.YWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b5/43/b543af57-7466-4c68-8a21-427b00e7cbd6/swz48_pilot-r2-d2.png";

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R2D2PilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterOwnTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterOwnTrigger;
        }

        private void RegisterOwnTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, TryAssignCalculateToken);
        }

        private void TryAssignCalculateToken(object sender, EventArgs e)
        {
            bool isEnemyShipInRearSector = false;

            foreach (GenericShip enemyShip in HostShip.Owner.AnotherPlayer.Ships.Values)
            {
                if (HostShip.SectorsInfo.IsShipInSector(enemyShip, Arcs.ArcType.Rear))
                {
                    isEnemyShipInRearSector = true;
                    break;
                }
            }

            if (isEnemyShipInRearSector)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": 1 Calculate token is gained");
                Sounds.PlayShipSound("R2D2-Proud");
                HostShip.Tokens.AssignToken(typeof(Tokens.CalculateToken), Triggers.FinishTrigger);
            }
            else
            {
                Messages.ShowInfoToHuman(HostShip.PilotInfo.PilotName + ": No enemy ships in rear sector");
                Triggers.FinishTrigger();
            }
        }
    }
}
