using System;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class DataSourceLocationAttribute:Attribute
    {
        public DataSourceLocationAttribute(string dataSourceLocation)
        {
            DataSourceLocation = dataSourceLocation;
        }

        public string DataSourceLocation { get; set; }
    }
}
