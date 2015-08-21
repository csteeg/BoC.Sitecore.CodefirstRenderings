using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public abstract class ActionAttribute: Attribute
    {
        public abstract IEnumerable<KeyValuePair<ID, string>> GetFields();
    }
}