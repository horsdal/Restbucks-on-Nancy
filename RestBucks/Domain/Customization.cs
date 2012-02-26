using System.Collections.Generic;
using RestBucks.Domain.BaseClass;

namespace RestBucks.Domain
{
    public class Customization : EntityBase
    {
        private readonly ISet<string> possibleValues;
        
        public Customization()
        {
            possibleValues = new HashSet<string>();
        }

        public virtual string Name { get; set; }

        public virtual ISet<string> PossibleValues
        {
            get { return possibleValues; }
        }
    }
}