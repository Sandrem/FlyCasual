using System;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.M3AInterceptor
    {
        public class GenesisRed : M3AInterceptor
        {
            public GenesisRed() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Genesis Red",
                    7,
                    19,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.GenesisRedAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Genesis Red";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //After you aquire a target lock, assign focus and evade tokens to your ship until you have the same number of each token as the locked ship.
    public class GenesisRedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTargetLockIsAcquired -= RegisterTrigger;
        }

        private void RegisterTrigger(Ship.GenericShip target)
        {
            RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, delegate { GenesisRedAbilityEffect(target); });
        }

        private void GenesisRedAbilityEffect(Ship.GenericShip target)
        {
            AssignTokensIfLessThanTarget(target, typeof(FocusToken), () => new FocusToken(HostShip), delegate
            {
                AssignTokensIfLessThanTarget(target, typeof(EvadeToken), () => new EvadeToken(HostShip), Triggers.FinishTrigger);
            });
        }

        private void AssignTokensIfLessThanTarget(Ship.GenericShip target, Type tokenType, Func<GenericToken> createToken, Action callback)
        {
            if (HostShip.Tokens.CountTokensByType(tokenType) < target.Tokens.CountTokensByType(tokenType))
            {
                HostShip.Tokens.AssignToken(createToken(), delegate { AssignTokensIfLessThanTarget(target, tokenType, createToken, callback); });
            }
            else
            {
                callback();
            }
        }
    }
}