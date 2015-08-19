using System;

namespace BoC.Sitecore.CodeFirstRenderings.DataProviders
{
    public class ExperienceEditorButtonsAttribute : Attribute
    {
        public string ExperienceEditorButtons { get; set; }

        public ExperienceEditorButtonsAttribute(string experienceEditorButtons)
        {
            this.ExperienceEditorButtons = experienceEditorButtons;
        }
    }
}