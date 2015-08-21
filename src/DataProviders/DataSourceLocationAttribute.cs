using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class DataSourceLocationAttribute: ActionAttribute
    {
        public static readonly ID DataSourceLocationField = new ID("{B5B27AF1-25EF-405C-87CE-369B3A004016}");
        public DataSourceLocationAttribute(string dataSourceLocation)
        {
            DataSourceLocation = dataSourceLocation;
        }

        public string DataSourceLocation { get; set; }
        public override IEnumerable<KeyValuePair<ID, string>> GetFields()
        {
            yield return new KeyValuePair<ID, string>(DataSourceLocationField, DataSourceLocation);
        }
    }
}
