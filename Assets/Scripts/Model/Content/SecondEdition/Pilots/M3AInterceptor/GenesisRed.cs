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
                PilotInfo = new PilotCardInfo(
                    "Genesis Red",
                    4,
                    35,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.GenesisRedAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 184;
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

        private void RegisterTrigger(GenericShip target)
        {
            RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, delegate { GenesisRedAbilityEffect(target); });
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