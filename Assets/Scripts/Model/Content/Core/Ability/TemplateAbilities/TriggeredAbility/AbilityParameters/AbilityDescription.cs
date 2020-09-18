using Arcs;
using Ship;
using System.Collections.Generic;

namespace Abilities.Parameters
{
    public class AbilityDescription
    {
        public string Name { get; }
        public string Description { get; }
        public IImageHolder ImageSource { get; }

        public AbilityDescription(string name, string description, IImageHolder imageSource)
        {
            Name = name;
            Description = description;
            ImageSource = imageSource;
        }
    }
}
