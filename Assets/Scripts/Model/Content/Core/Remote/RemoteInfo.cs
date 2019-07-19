using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Remote
{
    public class RemoteInfo
    {
        public string Name { get; }

        public int Initiative { get; }
        public int Agility { get; }
        public int Hull { get; }

        public string ImageUrl { get; }

        public Type Ability { get; }
        public Type SystemPhaseAbility { get; }

        public RemoteInfo(string name, int initiative, int agility, int hull, string imageUrl, Type ability = null, Type systemPhaseAbility = null)
        {
            Name = name;

            Initiative = initiative;
            Agility = agility;
            Hull = hull;

            ImageUrl = imageUrl;

            Ability = ability;
            SystemPhaseAbility = systemPhaseAbility;
        }
    }
}
