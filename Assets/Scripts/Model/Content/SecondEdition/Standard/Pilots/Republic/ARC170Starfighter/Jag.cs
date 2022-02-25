using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class Jag : ARC170Starfighter
        {
            public Jag() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Jag\"",
                    "CT-55/11-9009",
                    Faction.Republic,
                    3,
                    5,
                    14,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone 
                    },
                    abilityType: typeof(Abilities.SecondEdition.JagAbility),
                    skinName: "Red"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/06/ec/06ecf59a-74bb-425b-9b5e-0d90a76d3261/swz33_jag.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 1-2 in your left or right arc defends, you may acquire a lock on the attacker.
    public class JagAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += CheckAbility;
        }
        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
    {
            var rangeLeft = HostShip.SectorsInfo.RangeToShipBySector(Combat.Defender, Arcs.ArcType.Left);
            var rangeRight = HostShip.SectorsInfo.RangeToShipBySector(Combat.Defender, Arcs.ArcType.Right);

            if (Combat.Defender.Owner == HostShip.Owner
                && ((HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Left) && rangeLeft >= 1 && rangeLeft <= 2)
                    || (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Right) && rangeRight >= 1 && rangeRight <= 2)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, delegate
                {
                    AskToUseAbility(
                        HostShip.PilotInfo.PilotName,
                        AlwaysUseByDefault,
                        AcquireLock,
                        descriptionLong: "Do you want to acquire a lock on the attacker?",
                        imageHolder: HostShip
                    );
                });                
            }
        }

        private void AcquireLock(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostName + ": Acquires lock on " + Combat.Attacker.PilotName);
            ActionsHolder.AcquireTargetLock(HostShip, Combat.Attacker, SubPhases.DecisionSubPhase.ConfirmDecision, SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}