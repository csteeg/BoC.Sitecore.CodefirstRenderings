<?xml version="1.0" encoding="utf-8" ?>
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <dataProviders>
      <!-- use this line instead of the default one, if you use Glass in your projects. Also uncomment the code in DataProviders/GlassCodeFirstRenderingsDataProvider.cs 
          <CodeFirstRenderingsDataProvider type="BoC.Sitecore.CodeFirstRenderings.DataProviders.GlassCodeFirstRenderingsDataProvider, $AssemblyName$">
      -->
      <CodeFirstRenderingsDataProvider type="BoC.Sitecore.CodeFirstRenderings.DataProviders.CodeFirstRenderingsDataProvider, BoC.Sitecore.CodeFirstRenderings">
        <namespaces hint="list:AddNamespace">
          <namespace>$rootnamespace$</namespace>
        </namespaces>
      </CodeFirstRenderingsDataProvider>
    </dataProviders>
    <databases>
      <database id="master">
        <dataProviders hint="list:AddDataProvider">
          <dataProvider ref="dataProviders/CodeFirstRenderingsDataProvider"/>
        </dataProviders>
      </database>
      <database id="web">
        <dataProviders hint="list:AddDataProvider">
          <dataProvider ref="dataProviders/CodeFirstRenderingsDataProvider"/>
        </dataProviders>
      </database>
    </databases>
  </sitecore>
</configuration>