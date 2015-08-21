using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class DataSourceTemplateAttribute: ActionAttribute
    {
        public static readonly ID DataSourceTemplateField = new ID("{1A7C85E5-DC0B-490D-9187-BB1DBCB4C72F}");

        public DataSourceTemplateAttribute(string dataSourceTemplate)
        {
            DataSourceTemplate = dataSourceTemplate;
        }

        public string DataSourceTemplate { get; set; }
        public override IEnumerable<KeyValuePair<ID, string>> GetFields()
        {
            yield return new KeyValuePair<ID, string>(DataSourceTemplateField, DataSourceTemplate);
        }
    }
}
