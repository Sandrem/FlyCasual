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

}
