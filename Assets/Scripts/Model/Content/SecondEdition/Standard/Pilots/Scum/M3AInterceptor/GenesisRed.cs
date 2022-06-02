using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class GenesisRed : M3AInterceptor
        {
            public GenesisRed() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Genesis Red",
                    "Tansarii Point Crime Lord",
                    Faction.Scum,
                    4,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GenesisRedAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    seImageNumber: 184
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
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

        private void RegisterTrigger(ITargetLockable target)
        {
            if (target is GenericShip)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, delegate { GenesisRedAbilityEffect(target as GenericShip); });
            }
        }

        private void GenesisRedAbilityEffect(GenericShip target)
        {
            List<GenericToken> targetTokens = target.Tokens.GetAllTokens();
            int focusTokens = targetTokens.Where(token => token is FocusToken).Count();
            int evadeTokens = targetTokens.Where(token => token is EvadeToken).Count();

            // welcome to hell
            HostShip.Tokens.RemoveAllTokensByType(typeof(FocusToken), delegate
            {
                HostShip.Tokens.RemoveAllTokensByType(typeof(EvadeToken), delegate
                {
                    HostShip.Tokens.AssignTokens(
                        () => new FocusToken(HostShip),
                        focusTokens,
                        delegate
                        {
                            HostShip.Tokens.AssignTokens(
                                () => new EvadeToken(HostShip),
                                evadeTokens,
                                Triggers.FinishTrigger);
                        });
                });
            });
        }
    }
}