using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sitecore.Data;
using System.Web.Mvc;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class ControllerType
    {
        private ControllerType(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }

        private string _description;
        public string Description
        {
            get
            {
                if (_description == null)
                {
                    var attributes = Type.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                    {
                        _description = ((DescriptionAttribute)attributes[0]).Description;
                        if (_description.Contains("{0}"))
                            _description = String.Format(_description, ControllerName);
                    }
                    if (String.IsNullOrEmpty(_description))
                        _description = ControllerName;
                }
                return _description;
            }
        }

        public string ControllerName
        {
            get
            {
                if (Type.Name.EndsWith("Controller", StringComparison.CurrentCultureIgnoreCase))
                {
                    return Type.Name.Substring(0, Type.Name.Length - "controller".Length);
                }
                return Type.Name;
            }
        }

        private Guid? _id;
        public Guid Id
        {
            get
            {
                return (Guid)(_id ?? (_id = ToUniqueId(Type.AssemblyQualifiedName)));
            }
        }

        private Guid? _parentId;
        public Guid ParentId
        {
            get
            {
                return (Guid)(_parentId ?? (_parentId = ToUniqueId(Type.Namespace)));
            }
        }


        #region static helpers
        internal static Guid ToUniqueId(string input)
        {
            //this code will generate a unique Guid for a string (unique with a 2^20.96 probability of a collision) 
            //http://stackoverflow.com/questions/2190890/how-can-i-generate-guid-for-a-string-values
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }

        public static ControllerType GetControllerType(ID id)
        {
            ControllerType controller = null;
            GetAllControllers().TryGetValue(id.ToGuid(), out controller);
            return controller;
        }

        private static readonly object _allNamespacesLock = new object();
        internal static IDictionary<Guid, string> _allNamespaces;

        internal static IDictionary<Guid, string> GetAllNamespaces()
        {
            if (_allNamespaces != null)
                return _allNamespaces;
            lock (_allNamespacesLock)
            {
                if (_allNamespaces != null)
                    return _allNamespaces;
                return _allNamespaces = GetAllControllers().Values
                    .Select(ct => ct.Type.Namespace)
                    .Distinct()
                    .ToDictionary(ToUniqueId);
            }
        }

        private static readonly object _allControllersLock = new object();
        internal static IDictionary<Guid, ControllerType> _allControllers;

        internal static IDictionary<Guid,ControllerType> GetAllControllers()
        {
            if (_allControllers != null)
                return _allControllers;
            lock (_allControllersLock)
            {
                if (_allControllers != null)
                    return _allControllers;

                return _allControllers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes().Where(t =>
                            !t.IsAbstract &&
                            !t.IsInterface &&
                            t.IsClass &&
                            (!CodeFirstRenderingsDataProvider.TypeFilters.Any() || 
                            CodeFirstRenderingsDataProvider.TypeFilters.Any(func => func(t))) &&
                            typeof (IController).IsAssignableFrom(t)).ToList();
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                }).Select(t => new ControllerType(t)).ToDictionary(t => t.Id);
            }
        }
        #endregion

        public static void Clear()
        {
            lock (_allControllersLock)
                lock (_allNamespacesLock)
                {
                    _allControllers = null;
                    _allNamespaces = null;
                }
        }
    }
}