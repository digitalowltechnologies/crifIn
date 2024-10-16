using System;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    public class RadioButtonListBoxed : Umbraco.Forms.Core.Providers.FieldTypes.RadioButtonList
    {
        public RadioButtonListBoxed() : base()
        {
            base.Id = Guid.Parse("A1ED5406-6660-413A-8704-6AF432654A50");

            base.Name = "Single choice with box style";
            base.Description = "Renders a radio button list to enable a single choice answer";
            Icon = "icon-target";
            DataType = FieldDataType.String;
            Category = "List";
            SortOrder = 90;
            ShowLabel = "True";
            FieldTypeViewName = "FieldType.RadioButtonListBoxed.cshtml";
        }
    }
}