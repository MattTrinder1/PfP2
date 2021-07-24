using Common.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Models.Base
{
    public class DVBase
    {
        private Guid? id = null;

        public DVBase()
        {
        }

        [JsonIgnore]
        public Guid? Id {
            get
            {
                return id;
            }

            set
            {
                if (value != Guid.Empty)
                {
                    id = value;
                }
                else
                {
                    value = null;
                }
            }
        }
    }
}
