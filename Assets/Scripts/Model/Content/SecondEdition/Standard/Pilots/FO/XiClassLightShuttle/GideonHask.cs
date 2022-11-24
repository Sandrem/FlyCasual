using System.Collections.Generic;
using BoardTools;
using Upgrade;
using Ship;
using System;

namespace Ship
{
    namespace SecondEdition.XiClassLightShuttle
    {
        public class GideonHask : XiClassLightShuttle
        {
            public GideonHask() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gideon Hask",
                    "Merciless Hard-Liner",
                    Faction.FirstOrder,
                    4,
                    4,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GideonHaskXiClassLightShuttleAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fc/47/fc47d552-1b3f-4281-8a0e-5ced929d7ec8/swz69_a1_ship_hask.png";

                PilotNameCanonical = "gideonhask-xiclasslightshuttle";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GideonHaskXiClassLightShuttleAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Add,
                1,
                sideCanBeChangedTo: DieSide.Unknown,
                isGlobal: true,
                payAbilityCost: GainStrainToken
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;
            if (Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo) return false;
            if (Combat.Attacker.ShipBase.Size != BaseSize.Small && Combat.Attacker.ShipId != HostShip.ShipId) return false;
            if (!Combat.Defender.Damage.IsDamaged) return false;
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return false;
            if (Combat.Attacker.DiceRolledLastAttack > 2) return false;
            if (new DistanceInfo(Combat.Attacker, HostShip).Range > 2) return false;

            return true;
        }

        private void GainStrainToken(Action<bool> callback)
        {
            Combat.Attacker.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                delegate { callback(true); }
            );
        }

        private int GetAiPriority()
        {
            return 110;
        }
    }
}

