using Content;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class ViktorHel : KihraxzFighter
        {
            public ViktorHel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Viktor Hel",
                    "Storied Bounty Hunter",
                    Faction.Scum,
                    4,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ViktorHelAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    seImageNumber: 193,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ViktorHelAbility : GenericAbility
    {
        GenericShip attacker;
        int defenseDiceRolled;

        public override void ActivateAbility()
        {
            HostShip.AfterNumberOfDefenceDiceConfirmed += StoreDiceRolled;
            HostShip.OnAttackFinishAsDefender += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterNumberOfDefenceDiceConfirmed -= StoreDiceRolled;
            HostShip.OnAttackFinishAsDefender -= RegisterAbility;
        }

        public void StoreDiceRolled(ref int dice)
        {
            attacker = Combat.Attacker;
            defenseDiceRolled = dice;
        }

        public void RegisterAbility(GenericShip ship)
        {
            if (defenseDiceRolled == 2)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AssignStress);
        }

        public void AssignStress(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " assigns the attacker a stress token");
            attacker.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}

