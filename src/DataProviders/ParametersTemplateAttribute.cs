using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class ParametersTemplateAttribute: ActionAttribute
    {
        public static readonly ID ParametersTemplateField = new ID("{13F89250-AD6B-4548-882E-118A12C18094}");
        public ParametersTemplateAttribute(string parametersTemplate)
        {
            ParametersTemplate = parametersTemplate;
        }

        public string ParametersTemplate { get; set; }
        public override IEnumerable<KeyValuePair<ID, string>> GetFields()
        {
            yield return new KeyValuePair<ID, string>(ParametersTemplateField, ParametersTemplate);
        }
    }
}
