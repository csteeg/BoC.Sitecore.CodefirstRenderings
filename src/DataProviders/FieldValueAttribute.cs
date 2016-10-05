using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class FieldValueAttribute : ActionAttribute
    {
        private readonly ID _fieldId;
        private readonly string _value;

        public FieldValueAttribute(string fieldId, string value)
        {
            _fieldId = new ID(fieldId);
            _value = value;
        }
        public override IEnumerable<KeyValuePair<ID, string>> GetFields()
        {
            if (_fieldId == ID.Null || string.IsNullOrEmpty(_value))
                yield break;
            
            yield return new KeyValuePair<ID, string>(_fieldId, _value);
        }
    }
}
