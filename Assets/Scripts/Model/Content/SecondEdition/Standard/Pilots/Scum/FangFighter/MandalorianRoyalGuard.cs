using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class MandalorianRoyalGuard : FangFighter
        {
            public MandalorianRoyalGuard() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Mandalorian Royal Guard",
                    "Unlikely Ally",
                    Faction.Scum,
                    4,
                    5,
                    10,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.MandalorianRoyalGuardAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Skull Squadron"
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/mandalorianroyalguard.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MandalorianRoyalGuardAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Mandalorian Royal Guard",
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                1,
                sideCanBeChangedTo: DieSide.Success,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );
        }
        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.DepleteToken), delegate { });
            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), () => callback(true));
        }

        public bool IsAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Tools.IsSameTeam(Combat.Defender, HostShip) && Combat.Defender.ShipInfo.BaseSize != BaseSize.Small)
            {
                ShotInfoArc arcInfo = new ShotInfoArc(
                    Combat.Attacker,
                    HostShip,
                    Combat.ArcForShot
                );

                if (arcInfo.InArc) result = true;
            }

            return result;
        }

        private int AiPriority()
        {
            return 95;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}