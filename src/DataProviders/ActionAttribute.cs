using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public abstract class ActionAttribute: Attribute
    {
        public abstract IEnumerable<KeyValuePair<ID, string>> GetFields();
    }
}