﻿using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.KihraxzFighter
    {
        public class TalonbaneCobra : KihraxzFighter
        {
            public TalonbaneCobra() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Talonbane Cobra",
                    9,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.TalonbaneCobraAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}


namespace Abilities.FirstEdition
{
    public class TalonbaneCobraAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += TalonbaneCobraDiceCheck;
            HostShip.AfterGotNumberOfDefenceDice += TalonbaneCobraDiceCheck;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= TalonbaneCobraDiceCheck;
            HostShip.AfterGotNumberOfDefenceDice -= TalonbaneCobraDiceCheck;
        }

        private void TalonbaneCobraDiceCheck(ref int diceCount)
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                if (Combat.AttackStep == CombatStep.Attack && Combat.ShotInfo.Range == 1)
                {
                    Messages.ShowInfo("Talonbane Cobra is attacking at range 1, gaining +1 attack die.");
                    diceCount++;
                }
                if (Combat.AttackStep == CombatStep.Defence && Combat.ShotInfo.Range == 3)
                {
                    Messages.ShowInfo("Talonbane Cobra is defending at range 3, gaining +1 defense die.");
                    diceCount++;
                }
            }
        }
    }
}
