using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter.Model
{
    /// <summary>
    /// Model, welches eine Zuordnung repräsentiert
    /// </summary>
    public class ZuordnungModel
    {
        //Property, welche den Text der TextBox entgegennimmt
        public string EntryText { get; set; }
        //Property für die Werte der ComboBox
        public ObservableCollection<string> DropDownEntries { get; set; } = new ObservableCollection<string>(StaticHelper.BestandteileList);
        //Property, welche den ausgewählten Eintrag der ComboBox repräsentiert
        public string SelectedDropDownEntry { get; set; } = string.Empty;
    }
}
