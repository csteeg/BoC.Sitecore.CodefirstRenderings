using System;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class DataSourceTemplateAttribute: Attribute
    {
        public DataSourceTemplateAttribute(string dataSourceTemplate)
        {
            DataSourceTemplate = dataSourceTemplate;
        }

        public string DataSourceTemplate { get; set; }
    }
}
