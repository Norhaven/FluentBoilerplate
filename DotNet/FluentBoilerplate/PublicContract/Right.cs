using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    public class Right:IRight
    {
        public int Id { get; private set; }
        public string Description { get; private set; }

        public Right(int id, string description)
        {
            this.Id = id;
            this.Description = description;
        }
    }
}
