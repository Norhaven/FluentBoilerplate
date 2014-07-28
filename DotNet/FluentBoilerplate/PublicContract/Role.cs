using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    public class Role:IRole
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public IImmutableSet<IRight> Rights { get; private set; }

        public Role(int id, string description, IImmutableSet<IRight> rights)
        {
            this.Id = id;
            this.Description = description;
            this.Rights = rights;
        }
    }
}
