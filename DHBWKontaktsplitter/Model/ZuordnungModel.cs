using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter.Model
{
    public class ZuordnungModel
    {
        public string EntryText { get; set; }
        //public string LabelText { get; set; }
        public ObservableCollection<string> DropDownEntries { get; set; } = new ObservableCollection<string>(StaticHelper.BestandteileList);
        public string SelectedDropDownEntry { get; set; } = string.Empty;
    }
}
