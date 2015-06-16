using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Version = Sitecore.Data.Version;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class CodeFirstRenderingsDataProvider : DataProvider
    {
        protected static readonly ID ControllerRenderingId = new ID("{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}");

        private ID masterId = ID.Null;
        public virtual ID MasterId
        {
            get { return masterId; }
            set { masterId = value; }
        }

        private ID _parentId = new ID("{32566F0E-7686-45F1-A12F-D7260BD78BC3}");
        public virtual ID ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        private ID _folderTemplateId = new ID("{93227C5D-4FEF-474D-94C0-F252EC8E8219}"); ///sitecore/templates/System/Layout/Layout Folder
        public virtual ID FolderTemplateId
        {
            get { return _folderTemplateId; }
            set { _folderTemplateId = value; }
        }

        private ID _folderId = new ID("{11111111-1EE3-4181-A7E8-DFC489EAB2C4}");
        public virtual ID FolderId
        {
            get { return _folderId; }
            set { _folderId = value; }
        }

        private string _folderName = "MVC Actions";
        public virtual string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; }
        }


        #region fieldids

        private static class FieldIds
        {
            public static readonly ID ControllerName = new ID("{E64AD073-DFCC-4D20-8C0B-FE5AA6226CD7}");
            public static readonly ID ControllerAction = new ID("{DED9E431-3604-4921-A4B3-A6EC7636A5B6}");
            public static readonly ID DataSourceTemplate = new ID("{1A7C85E5-DC0B-490D-9187-BB1DBCB4C72F}");
            public static readonly ID DataSourceLocation = new ID("{B5B27AF1-25EF-405C-87CE-369B3A004016}");
        }

        #endregion

        public static IList<Func<Type, bool>> TypeFilters = new ObservableCollection<Func<Type, bool>>();

        public void AddNamespace(string nspace)
        {
            TypeFilters.Add(type => type.Namespace != null && type.Namespace.StartsWith(nspace));
        }

        static CodeFirstRenderingsDataProvider()
        {
            ((INotifyCollectionChanged)TypeFilters).CollectionChanged += (sender, args) =>
            {
                ControllerAction.Clear();
                ControllerType.Clear();
            };
        }

        public override bool SaveItem(ItemDefinition itemDefinition, ItemChanges changes, CallContext context)
        {
            var sqlProvider = GetSqlProvider(Database);
            if (!EnsureSqlVersion(itemDefinition, sqlProvider, context))
                return false;

            sqlProvider.SaveItem(itemDefinition, changes, context);
            return true;
        }

        public override bool CreateItem(ID itemID, string itemName, ID templateID, ItemDefinition parent, CallContext context)
        {
            var sqlProvider = GetSqlProvider(Database);
            return EnsureSqlVersion(parent, sqlProvider, context) &&
                    sqlProvider.CreateItem(itemID, itemName, templateID, parent, context);
        }

        private bool EnsureSqlVersion(ItemDefinition itemDefinition, DataProvider sqlProvider, CallContext context)
        {
            var sqlVersion = GetSqlVersion(itemDefinition.ID, context, sqlProvider);
            if (sqlVersion != null)
            {
                return true;
            }
            if (itemDefinition.ID == FolderId ||
                ControllerType.GetAllNamespaces().ContainsKey(itemDefinition.ID.ToGuid())
                || ControllerType.GetAllControllers().ContainsKey(itemDefinition.ID.ToGuid())
                || ControllerAction.GetControllerAction(itemDefinition.ID) != null)
            {
                var parentId = GetParentID(itemDefinition, context) ?? sqlProvider.GetParentID(itemDefinition, context);
                var itemdef = GetItemDefinition(parentId, context) ?? sqlProvider.GetItemDefinition(parentId, context);
                if (!sqlProvider.CreateItem(itemDefinition.ID, itemDefinition.Name, itemDefinition.TemplateID, itemdef, context))
                {
                    return false;
                }
                
                var item = Database.GetItem(itemDefinition.ID);
                var existingFields = new ItemChanges(item);
                foreach (Field field in item.Fields)
                {
                    existingFields.SetFieldValue(field, item[field.ID]);
                }
                sqlProvider.SaveItem(itemDefinition, existingFields, context);
                return true;
            }
            return false;
        }

        private ItemDefinition GetSqlVersion(ID itemId, CallContext context, DataProvider sqlProvider)
        {
            if (sqlProvider == null)
                return null;
            var sqlVersion = sqlProvider.GetItemDefinition(itemId, context);
            return sqlVersion;
        }

        /// <summary>
        /// Gets the base SQL provider that will store physical items
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private DataProvider GetSqlProvider(Database db)
        {
            var providers = db.GetDataProviders();
            return providers.FirstOrDefault(x => x is SqlDataProvider);
        }

        public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
        {
            Assert.ArgumentNotNull(itemDefinition, "itemDefinition");
            Assert.ArgumentNotNull(context, "context");

            if (GetSqlVersion(itemDefinition.ID, context, GetSqlProvider(Database)) != null)
                return null;
            if (itemDefinition.ID == FolderId ||
                ControllerType.GetAllNamespaces().ContainsKey(itemDefinition.ID.ToGuid())
                || ControllerType.GetAllControllers().ContainsKey(itemDefinition.ID.ToGuid())
                || ControllerAction.GetControllerAction(itemDefinition.ID) != null)
            {

                VersionUriList versionUriList = new VersionUriList();
                foreach (Language language in LanguageManager.GetLanguages(Database))
                    versionUriList.Add(language, Version.First);
                return versionUriList;
            }
            return null;
        }

        public override LanguageCollection GetLanguages(CallContext context)
        {
            return null;
        }

        public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
        {
            Assert.ArgumentNotNull(itemId, "itemId");
            Assert.ArgumentNotNull(context, "context");
            if (GetSqlVersion(itemId, context, GetSqlProvider(Database)) != null)
                return null;
            
            if (itemId == FolderId)
                return new ItemDefinition(itemId, FolderName, FolderTemplateId, this.MasterId);
            

            var allNamespaces = ControllerType.GetAllNamespaces();
            var nspace = allNamespaces.ContainsKey(itemId.ToGuid()) ? allNamespaces[itemId.ToGuid()] : null;
            if (!string.IsNullOrEmpty(nspace))
            {
                return new ItemDefinition(itemId, nspace, FolderTemplateId, this.MasterId);
            }

            var type = ControllerType.GetControllerType(itemId);
            if (type != null)
            {
                return new ItemDefinition(itemId, type.ControllerName, FolderTemplateId, this.MasterId);
            }

            var action = ControllerAction.GetControllerAction(itemId);
            if (action != null)
            {
                return new ItemDefinition(itemId, action.ActionName, ControllerRenderingId, this.MasterId);
            }

            return base.GetItemDefinition(itemId, context);
        }

        public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
        {
            if (GetSqlVersion(itemDefinition.ID, context, GetSqlProvider(Database)) != null)
                return null;

            if (itemDefinition.ID == FolderId)
                return ParentId;

            if (ControllerType.GetAllNamespaces().ContainsKey(itemDefinition.ID.ToGuid()))
            {
                return FolderId;
            }

            ControllerType controller = ControllerType.GetControllerType(itemDefinition.ID);
            if (controller != null)
            {
                return new ID(controller.ParentId);
            }

            ControllerAction action = ControllerAction.GetControllerAction(itemDefinition.ID);
            if (action != null)
            {
                return new ID(action.ControllerType.Id);
            }

            return base.GetParentID(itemDefinition, context);
        }

        public override FieldList GetItemFields(ItemDefinition item, VersionUri version, CallContext context)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(version, "version");
            Assert.ArgumentNotNull(context, "context");

            if (GetSqlVersion(item.ID, context, GetSqlProvider(Database)) != null)
                return null;

            var list = new FieldList();
            ControllerType controllerType = null;
            if (item.ID == FolderId || ControllerType.GetAllNamespaces().ContainsKey(item.ID.ToGuid()) || ((controllerType = ControllerType.GetControllerType(item.ID)) != null))
            {
                list.Add(FieldIDs.Icon, Themes.MapTheme("SoftwareV2/16x16/elements.png"));
                if (controllerType != null)
                {
                    list.Add(FieldIDs.DisplayName, controllerType.Description);
                }
            }
            else
            {
                var action = ControllerAction.GetControllerAction(item.ID);
                if (action != null && HttpContext.Current != null)
                {
                    AddActionFields(list, action);
                }
            }
            if (list.Count == 0)
                return base.GetItemFields(item, version, context);

            return list;
        }

        protected virtual void AddActionFields(FieldList list, ControllerAction action)
        {
            list.Add(FieldIDs.Icon, Themes.MapTheme("SoftwareV2/16x16/element.png"));
            list.Add(FieldIDs.DisplayName, action.Description);
            list.Add(FieldIds.ControllerName, action.ControllerType.ControllerName);
            list.Add(FieldIds.ControllerAction, action.ActionName);

            var dataSourceTemplateAttribute = action.MethodInfo.GetCustomAttribute<DataSourceTemplateAttribute>();
            if (dataSourceTemplateAttribute != null)
            {
                list.Add(FieldIds.DataSourceTemplate, dataSourceTemplateAttribute.DataSourceTemplate);
            }

            var dataSourceLocationAttribute = action.MethodInfo.GetCustomAttribute<DataSourceLocationAttribute>();
            if (dataSourceLocationAttribute != null)
            {
                list.Add(FieldIds.DataSourceLocation, dataSourceLocationAttribute.DataSourceLocation);
            }
        }

        public override bool MoveItem(ItemDefinition itemDefinition, ItemDefinition destination, CallContext context)
        {
            var sqlProvider = GetSqlProvider(Database);
            if (!EnsureSqlVersion(itemDefinition, sqlProvider, context)) 
                return false;
            if (!EnsureSqlVersion(destination, sqlProvider, context))
                return false;
            
            sqlProvider.MoveItem(itemDefinition, destination, context);
            return true;
        }


        public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
        {
            ControllerType controllerType;
            var list = new IDList();

            if (itemDefinition.ID == ParentId)
            {
                if (GetSqlVersion(FolderId, context, GetSqlProvider(Database)) == null)
                    list.Add(FolderId);
            }
            else if (itemDefinition.ID == FolderId)
            {
                AddAllNamespaces(list, context);
            }
            else if (ControllerType.GetAllNamespaces().ContainsKey(itemDefinition.ID.ToGuid()))
            {
                AddControllers(list, itemDefinition.ID.ToGuid(), context);
            }
            else if ((controllerType = ControllerType.GetControllerType(itemDefinition.ID)) != null)
            {
                AddAllActions(list, controllerType, context);
            }
            else
            {
                return null;
            }

            if (list.Count == 0)
                return null;
            return list;
        }

        void AddAllActions(IDList list, ControllerType controllerType, CallContext context)
        {
            var sqlProvider = GetSqlProvider(Database);
            foreach (var action in ControllerAction.GetAllActions(controllerType.Id).Where(a =>
            {
                return GetSqlVersion(new ID(a.Id), context, sqlProvider) == null;
            }))
            {
                if (!list.Contains(new ID(action.Id)))
                    list.Add(new ID(action.Id));
            }
        }

        void AddControllers(IDList list, Guid parentId, CallContext context)
        {
            var sqlProvider = GetSqlProvider(Database);
            foreach (var controller in ControllerType.GetAllControllers().Values.Where(c =>
            {
                if (c.ParentId != parentId)
                    return false;
                return GetSqlVersion(new ID(c.Id), context, sqlProvider) == null;
            }))
            {
                list.Add(new ID(controller.Id));
            }
        }

        void AddAllNamespaces(IDList list, CallContext context)
        {
            var sqlProvider = GetSqlProvider(Database);
            foreach (var nspace in ControllerType.GetAllNamespaces().Where(s =>
            {
                return GetSqlVersion(new ID(s.Key), context, sqlProvider) == null;
            }))
            {
                list.Add(new ID(nspace.Key));
            }
        }
    }
}