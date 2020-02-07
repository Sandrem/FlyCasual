﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.Fireball
    {
        public class JarekYeager : Fireball
        {
            public JarekYeager() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jarek Yeager",
                    5,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JarekYeagerAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/cc580fd073ea51094b881e37775ef1f0.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you have 2 or fewer stress tokens, if you are damaged, you can execute red basic maneuvers even while stressed;
    //if you are critically damaged, you can execute red advanced maneuvers even while stressed.
    public class JarekYeagerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed += CheckRedManeuversWhileStressed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed -= CheckRedManeuversWhileStressed;
        }

        private void CheckRedManeuversWhileStressed(ref bool isAllowed)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) <= 2 && HostShip.Damage.IsDamaged())
            {
                bool isCriticallyDamaged = HostShip.Damage.HasFaceupCards;
                bool isAdvancedManeuver = HostShip.AssignedManeuver != null && HostShip.AssignedManeuver.IsAdvancedManeuver;

                if (isAdvancedManeuver && !isCriticallyDamaged) return;

                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Red maneuver is allowed");
                isAllowed = true;
            }
        }
    }
}