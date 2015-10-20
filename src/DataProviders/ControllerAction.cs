using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Sitecore.Data;
using System.Web.Mvc;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class ControllerAction : IComparable<ControllerAction>
    {
        public ControllerType ControllerType { get; private set; }
        public string ActionName { get; private set; }
        public Guid ParentId { get { return ControllerType.Id; } }

        private Guid? _id;
        public Guid Id
        {
            get
            {
                if (_id != null)
                    return _id.Value;
                var attributes = MethodInfo.GetCustomAttributes(typeof(ControllerActionAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                   return (_id = ((ControllerActionAttribute)attributes[0]).Id).Value;
                }

                return (Guid) (_id ?? (_id = ControllerType.ToUniqueId(ActionName + ControllerType.Type.AssemblyQualifiedName)));
            }
        }
        public MethodInfo MethodInfo { get; set; }
        private string _description;
        public string Description
        {
            get
            {
                if (_description == null)
                {
                    var attributes = MethodInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        _description = ((DescriptionAttribute)attributes[0]).Description;
                        if (_description.Contains("{0}"))
                            _description = String.Format(_description, ActionName);
                    }
                    if (String.IsNullOrEmpty(_description))
                        _description = string.Format("{0} - {1}", ControllerType.ControllerName, ActionName);
                }
                return _description;
            }
        }

        public int CompareTo(ControllerAction other)
        {
            if (other == null)
                return -1;

            return String.CompareOrdinal(ActionName, other.ActionName);
        }

        #region static helpers
        internal static string GetKey(Type controllerType, string actionName)
        {
            return string.Format("{0}/{1}", controllerType.FullName, actionName);
        }

        public static ControllerAction GetControllerAction(ID itemId)
        {
            return GetAllActions().Values.SelectMany(set => set).FirstOrDefault(action => action.Id == itemId.ToGuid());
        }

        public static IEnumerable<ControllerAction> GetAllActions(Guid parentId)
        {
            var allActions = GetAllActions();
            if (!allActions.ContainsKey(parentId))
            {
                return Enumerable.Empty<ControllerAction>();
            }
            return allActions[parentId];
        }

        private static readonly object _allActionsLock = new object();
        internal static IDictionary<Guid, ISet<ControllerAction>> _allActions = new Dictionary<Guid, ISet<ControllerAction>>();

        private ControllerAction(string name, ControllerType controllerType, MethodInfo methodInfo)
        {
            ActionName = name;
            ControllerType = controllerType;
            MethodInfo = methodInfo;
        }

        private static IDictionary<Guid, ISet<ControllerAction>> GetAllActions()
        {
            if (_allActions.Any()) return _allActions;
            
            lock (_allActionsLock)
            {
                if (_allActions.Any()) return _allActions;

                var controllers = ControllerType.GetAllControllers().Values;
                foreach (var controllerType in controllers)
                {
                    var controllerId = ControllerType.ToUniqueId(controllerType.Type.AssemblyQualifiedName);
                    if (!_allActions.ContainsKey(controllerId))
                        _allActions[controllerId] = new SortedSet<ControllerAction>();

                    MethodInfo[] allMethods = controllerType.Type.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
                    MethodInfo[] actionMethods = Array.FindAll(allMethods, IsValidActionMethod);

                    var aliasedMethods = Array.FindAll(actionMethods, info => info.IsDefined(typeof(ActionNameAttribute), true /* inherit */));
                    var nonAliasedMethods = actionMethods.Except(aliasedMethods);
                    foreach (var aliasedMethod in aliasedMethods)
                    {
                        var name = aliasedMethod.GetCustomAttributes(typeof(ActionNameAttribute), true).OfType<ActionNameAttribute>().FirstOrDefault();
                        if (name != null)
                        {
                            _allActions[controllerId].Add(new ControllerAction(name.Name, controllerType, aliasedMethod));
                        }
                    }
                    foreach (var method in nonAliasedMethods)
                    {
                        _allActions[controllerId].Add(new ControllerAction(method.Name,controllerType,method));
                    }
                }
            }
            return _allActions;
        }

        static bool IsValidActionMethod(MethodInfo methodInfo)
        {
            
            return !methodInfo.IsDefined(typeof(HttpPostAttribute)) && !methodInfo.IsSpecialName &&
                     !methodInfo.GetBaseDefinition().DeclaringType.IsAssignableFrom(typeof(Controller));
        }
        #endregion

        public static void Clear()
        {
            lock (_allActionsLock)
            {
                _allActions.Clear();
            }
        }
    }
}