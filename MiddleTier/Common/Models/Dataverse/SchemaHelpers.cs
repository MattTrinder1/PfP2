using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Dataverse
{
    public static class SchemaHelpers
    {
        public static string GetEntityPluralName(string entityName)
        {
            //todo exceptions to the general rule here

            return entityName + "s";
        }
    }
}
