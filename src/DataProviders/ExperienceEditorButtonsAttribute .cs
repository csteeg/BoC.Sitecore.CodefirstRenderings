using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class ExperienceEditorButtonsAttribute : ActionAttribute
    {
        public static readonly ID PageEditorButtonsField = new ID("{A2F5D9DF-8CBA-4A1D-99EB-51ACB94CB057}");
        public string ExperienceEditorButtons { get; set; }

        public ExperienceEditorButtonsAttribute(string experienceEditorButtons)
        {
            this.ExperienceEditorButtons = experienceEditorButtons;
        }

        public override IEnumerable<KeyValuePair<ID, string>> GetFields()
        {
            yield return new KeyValuePair<ID, string>(PageEditorButtonsField, ExperienceEditorButtons);
        }
    }
}