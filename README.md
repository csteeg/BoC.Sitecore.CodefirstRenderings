# BoC.Sitecore.CodefirstRenderings
One of the boring tasks when configuring a new sitecore website, 
is that you have to create the renderings for all controller actions you have created. 

This is the reason I’ve created a new data provider that adds all controller actions (or configurable what controller actions) 
to sitecore as a controller rendering. 

If you install the package [BoC.Sitecore.CodeFirstRenderings](https://www.nuget.org/packages/BoC.Sitecore.CodeFirstRenderings/) through nuget, 
you’ll get a dll reference, config file and a .cs file in your project

The file App_Config/Include/BoC/BoC.CodeFirstRenderings.config adds the default dataprovider to your current website. 
The default dataprovider scans all namespaces configured in this same config file (defaults to your project’s namespace) 
and adds them as controller renderings. 

If you don’t change anything, and add a controller like this: 

![image](http://www.chrisvandesteeg.nl/wp-content/uploads/2015/06/image2.png "image")

You’ll have these controller renderings available in sitecore:

![image](http://www.chrisvandesteeg.nl/wp-content/uploads/2015/06/image1.png "image")

As you can see, you can use several attributes on your actions as well. I think they all speak for themselves. 
You can add DataSourceLocation, DataSourceTemplate and Description (from System.ComponentModel) to specify where the 
DataSource-browse dialog should look for items, DataSourceTemplate to specify what template(s) to look 
for and a description to change the name of the controller rendering in sitecore. 

Now if you change anything to these controller renderings (eg rename, move, etc), they will be saved in the 
database and lose the codefirst functionality. This way, you can for example have them 
created by the codefirst provider on your development environment, and then move only the controller 
renderings you really want in your TAP environment to a different folder. You can then exclude the dataprovider on
your TAP environment (eg with [conditional configs](http://www.chrisvandesteeg.nl/2015/05/08/sitecore-conditional-configs-2-0/) :)) The file DataProviders/GlassCodeFirstRenderingsDataProvider.cs contains a class that has been commented out. If you use [Glass Mapper](http://glass.lu) as your choice of ORM,and you use the convention to name your action-parameter dataSource to retreive the strong typed datasource, you should enable this class. If you’ve removed the comments, you should also switch to using this by modifying the <CodeFirstRenderingsDataProvider> tag in App_Config/Include/BoC/BoC.CodeFirstRenderings.config  (the glass version has been commented out, so you should use that one)
