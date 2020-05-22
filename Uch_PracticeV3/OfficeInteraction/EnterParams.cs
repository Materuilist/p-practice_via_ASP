using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uch_PracticeV3.OfficeInteraction
{
    public class EnterParams
    {
        public string sheetName { get; }
        public List<string> colnames{ get; }
        public List<List<string>> rows { get; }

        public EnterParams(string sheetName, List<string> colnames, List<List<string>> rows)
        {
            this.sheetName = sheetName;
            this.colnames = colnames;
            this.rows = rows;
        }
    }
}
