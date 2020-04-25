using System.Collections;
using System.Collections.Generic;
using BoardTools;
using Arcs;
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
                PilotInfo = new PilotCardInfo(
                    "Gideon Hask",
                    4,
                    40,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GideonHaskXiClassLightShuttleAbility)
                );

                ImageUrl = "https://i.imgur.com/wfPdYgB.png";

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
            if (!Combat.Defender.Damage.IsDamaged()) return false;
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return false;
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

