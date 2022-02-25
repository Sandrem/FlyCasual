using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class GeonosianPrototype : HMPDroidGunship
        {
            public GeonosianPrototype() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Geonosian Prototype",
                    "Devastation Protocols",
                    Faction.Separatists,
                    2,
                    5,
                    18,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.GeonosianPrototypeAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.TacticalRelay
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/46/92/469279ec-b2cc-4168-81e2-f1bc616b0037/swz71_card_prototype.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GeonosianPrototypeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Geonosian Prototype",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                2,
                payAbilityCost: RemoveTractorToken
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.Cannon || Combat.ChosenWeapon.WeaponType == WeaponTypes.Missile)
            {
                if (Combat.Defender.Tokens.HasToken<TractorBeamToken>())
                {
                    result = true;
                }
            }

            return result;
        }

        private int GetAiPriority()
        {
            return 85;
        }

        private void RemoveTractorToken(Action<bool> callback)
        {
            if (Combat.Defender.Tokens.HasToken<TractorBeamToken>())
            {
                Combat.Defender.Tokens.RemoveToken
                (
                    typeof(TractorBeamToken),
                    delegate { callback(true); }
                );
            }
            else
            {
                callback(false);
            }            
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}