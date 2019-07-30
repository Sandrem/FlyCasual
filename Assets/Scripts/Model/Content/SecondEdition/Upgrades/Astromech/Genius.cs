﻿using Upgrade;
using SubPhases;
using Bombs;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class Genius : GenericUpgrade
    {
        public Genius() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Genius\"",
                UpgradeType.Astromech,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GeniusAbility),
                restriction: new FactionRestriction(Faction.Scum),                
                seImageNumber: 143
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GeniusAbility : FirstEdition.GeniusAbility
    {
        protected override string AbilityDescription => "Do you want to drop a bomb?";

        protected override UpgradeSubType BombTypeRestriction => UpgradeSubType.Bomb;

        protected override void StartDropBombSubphase()
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                Triggers.FinishTrigger
            );
        }
    }
}