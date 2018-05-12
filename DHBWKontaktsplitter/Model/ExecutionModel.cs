using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter.Model
{
    public class ExecutionModel
    {
        public bool HasError { get; set; } = false;
        public int ErrorId { get; set; }
        public int NotificationId { get; set; } = 0;
        public List<string> SplittedInput = new List<string>();
        public ContactModel Contact { get; set; } = new ContactModel();
    }
}
