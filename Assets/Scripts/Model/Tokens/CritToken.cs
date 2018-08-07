using Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tokens
{

    public class CritToken : GenericToken
    {
        public CritToken(GenericShip host) : base(host)
        {
            Name = "Critical Hit Token";
            Temporary = false;
            PriorityUI = 33;
        }
    }

    public class BlindedPilotCritToken : CritToken
    {
        public BlindedPilotCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://images-cdn.fantasyflightgames.com/filer_public/52/0d/520d9611-3a61-473d-8ebe-19c5659a9c6b/blinded-pilot.png";
        }
    }

    public class ConsoleFireCritToken : CritToken
    {
        public ConsoleFireCritToken(GenericShip host) : base(host)
        {
            Tooltip = "http://i.imgur.com/RwtlPpG.jpg";
        }
    }

    public class DamagedCockpitCritToken : CritToken
    {
        public DamagedCockpitCritToken(GenericShip host) : base(host)
        {
            Tooltip = "http://i.imgur.com/bNfmyKe.png";
        }
    }

    public class DirectHitCritToken : CritToken
    {
        public DirectHitCritToken(GenericShip host) : base(host)
        {
            Tooltip = "http://i.imgur.com/fjvsKRq.jpg";
        }
    }

    public class DamagedEngineCritToken : CritToken
    {
        public DamagedEngineCritToken(GenericShip host) : base(host)
        {
            Tooltip = "http://i.imgur.com/79SQIjR.jpg";
        }
    }

    public class DamagedSensorArrayCritToken : CritToken
    {
        public DamagedSensorArrayCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/6/61/Damaged-sensor-array.png";
        }
    }

    public class LooseStabilizerCritToken : CritToken
    {
        public LooseStabilizerCritToken(GenericShip host) : base(host)
        {
            Tooltip = "http://vignette4.wikia.nocookie.net/xwing-miniatures/images/d/db/Loose-stabilizer.png";
        }
    }

    public class MajorExplosionCritToken : CritToken
    {
        public MajorExplosionCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/a/ab/Major-explosion.png";
        }
    }

    public class MajorHullBreachCritToken : CritToken
    {
        public MajorHullBreachCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/f/f3/Major-hull-breach.png";
        }
    }

    public class ShakenPilotCritToken : CritToken
    {
        public ShakenPilotCritToken(GenericShip host) : base(host)
        {
            Tooltip = "http://vignette3.wikia.nocookie.net/xwing-miniatures/images/c/cf/Shaken-pilot.png";
        }
    }

    public class StructuralDamageCritToken : CritToken
    {
        public StructuralDamageCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://images-cdn.fantasyflightgames.com/filer_public/9b/2e/9b2ed16f-eaa5-4b3c-a9b5-682d5fb9cb2e/structural-damage-card.png";
        }
    }

    public class StunnedPilotCritToken : CritToken
    {
        public StunnedPilotCritToken(GenericShip host) : base(host)
        {
            Tooltip = "http://i.imgur.com/2ULmZZX.jpg";
        }
    }

    public class WeaponsFailureCritToken : CritToken
    {
        public WeaponsFailureCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/7/76/Swx36-weapons-failure.png";
        }
    }

    public class DisabledPowerRegulatorCritToken : CritToken
    {
        public DisabledPowerRegulatorCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/jj9Cv46.jpg";
        }
    }

    public class FuelLeakCritToken : CritToken
    {
        public FuelLeakCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/MNNWJJe.jpg";
        }
    }

    public class HullBreachCritToken : CritToken
    {
        public HullBreachCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/CIqxoU5.jpg";
        }
    }

    public class LooseStabilizerSECritToken : CritToken
    {
        public LooseStabilizerSECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/PMqkGn3.png";
        }
    }
    public class StructuralDamageSECritToken : CritToken
    {
        public StructuralDamageSECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/fJEhlAG.jpg";
        }
    }
    public class StunnedPilotSECritToken : CritToken
    {
        public StunnedPilotSECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/Cy4ovYs.jpg";
        }
    }
    public class WeaponsFailureSECritToken : CritToken
    {
        public WeaponsFailureSECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/BTd2jSS.png";
        }
    }

    public class WoundedPilotCritToken : CritToken
    {
        public WoundedPilotCritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/BIla4b2.jpg";
        }
    }

    public class DamagedSensorArraySECritToken : CritToken
    {
        public DamagedSensorArraySECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/6r6a8a7.jpg";
        }
    }

    public class DamagedEngineSECritToken : CritToken
    {
        public DamagedEngineSECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/sBDC3iQ.jpg";
        }
    }

    public class ConsoleFireSECritToken : CritToken
    {
        public ConsoleFireSECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/x4a6fqE.jpg";
        }
    }

    public class BlindedPilotSECritToken : CritToken
    {
        public BlindedPilotSECritToken(GenericShip host) : base(host)
        {
            Tooltip = "https://i.imgur.com/OoQBMf7.jpg";
        }
    }
}
