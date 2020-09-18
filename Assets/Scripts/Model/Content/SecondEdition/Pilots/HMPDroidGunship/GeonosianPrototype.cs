using Ship;
using System;
using Tokens;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class GeonosianPrototype : HMPDroidGunship
        {
            public GeonosianPrototype() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Geonosian Prototype",
                    2,
                    40,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.GeonosianPrototypeAbility)
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