using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class ControllerActionAttribute: Attribute
    {

        public ControllerActionAttribute(string id)
        {
            Guid templateId;
            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out templateId))
            {
                Id = templateId;
            }

        }

        public Guid Id { get; private set; }
    }
}