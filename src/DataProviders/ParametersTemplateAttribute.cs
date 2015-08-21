using System;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class ParametersTemplateAttribute: Attribute
    {
        public ParametersTemplateAttribute(string parametersTemplate)
        {
            ParametersTemplate = parametersTemplate;
        }

        public string ParametersTemplate { get; set; }
    }
}
