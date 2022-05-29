using System;
using System.Collections.Generic;

#nullable disable

namespace HelpdeskDAL
{
    public partial class Problem : HelpdeskEntity
    {
        public Problem()
        {
            Calls = new HashSet<Call>();
        }
        public string Description { get; set; }

        public virtual ICollection<Call> Calls { get; set; }
    }
}
