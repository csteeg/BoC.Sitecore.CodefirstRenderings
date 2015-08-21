/*
 * Remove the comments surrounding this code if you're using Glass in your project
 * This provider adds automatic datasource template id detection based on a parameter named datasource on your action
 * Also enable this provider in App_Config/Include/BoC/BoC.CodeFirstRenderings.config
 * 
using System;
using System.Linq;
using BoC.Sitecore.CodeFirstRenderings.DataProviders;
using Glass.Mapper;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class GlassCodeFirstRenderingsDataProvider : CodeFirstRenderingsDataProvider
    {
        private string _glassContextName = "Default";
        private static readonly ID DataSourceTemplateFieldId = new ID("{1A7C85E5-DC0B-490D-9187-BB1DBCB4C72F}");

        public string GlassContextName
        {
            get { return _glassContextName; }
            set { _glassContextName = value; }
        }

        protected override void AddActionFields(FieldList list, ControllerAction action)
        {
            base.AddActionFields(list, action);
            if (list[DataSourceTemplateFieldId] != null)
                return;
            var glassContext = Context.Contexts.ContainsKey(GlassContextName) ? Context.Contexts[GlassContextName] : null;
            if (glassContext == null)
                return;

            var parameters = action.MethodInfo.GetParameters();
            var datasourceParam =
                (from par in
                    parameters.Where(p => p.Name.Equals("datasource", StringComparison.InvariantCultureIgnoreCase))
                    select glassContext.GetTypeConfiguration<SitecoreTypeConfiguration>(par.ParameterType)
                    into glassConfig
                    where glassConfig != null
                    select glassConfig.TemplateId + "").ToList();
            if (datasourceParam.Any())
                list.Add(DataSourceTemplateFieldId, String.Join("|", datasourceParam));

            var templateParams =
                (from par in
                    parameters.Where(
                        p => p.Name.Equals("renderingParameters", StringComparison.InvariantCultureIgnoreCase))
                    select glassContext.GetTypeConfiguration<SitecoreTypeConfiguration>(par.ParameterType)
                    into glassConfig
                    where glassConfig != null
                    select glassConfig.TemplateId + "").ToList();
            if (templateParams.Any())
                list.Add(ParametersTemplateFieldId, String.Join("|", templateParams));
        }
    }
}
