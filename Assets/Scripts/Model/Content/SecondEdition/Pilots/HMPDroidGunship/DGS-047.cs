using Abilities.Parameters;
using Arcs;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class DGS047 : HMPDroidGunship
        {
            public DGS047() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "DGS-047",
                    1,
                    35,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Crew, UpgradeType.Device },
                    abilityType: typeof(Abilities.SecondEdition.DGS047Ability)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/27/e8/27e89f93-eaa8-40bc-b62f-d8d009e54b82/swz71_card_dgs047.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DGS047Ability : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterYouPerformAttack();

        public override AbilityPart Action => new SectorCheckAction
        (
            sectorType: ArcType.Front,
            targetShip: GetDefender,
            action: new AskAquireLockAction
            (
                description: new AbilityDescription
                (
                    name: "DGS-047",
                    description: "Do you want to acquire a lock on defender?",
                    imageSource: HostShip
                ),
                targetShip: GetDefender,
                showMessage: GetLockMessageToShow,
                action: new SectorCheckAction
                (
                    sectorType: ArcType.Bullseye,
                    targetShip: GetDefender,
                    action: new AssignTokenAction
                    (
                        tokenType: typeof(StrainToken),
                        targetShip: GetDefender,
                        showMessage: GetTokenMessageToShow
                    )
                )
            )
        );

        private GenericShip GetDefender()
        {
            return Combat.Defender;
        }

        private string GetLockMessageToShow()
        {
            return "DGS-047: Lock on " + Combat.Defender.PilotInfo.PilotName + " is aquired";
        }

        private string GetTokenMessageToShow()
        {
            return "DGS-047: Strain Token is assigned to " + Combat.Defender.PilotInfo.PilotName;
        }
    }
}