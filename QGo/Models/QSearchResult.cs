using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QGo.Models
{
    public class QSearchResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public QSearchResult()
        {
            Success = false;
            Message = string.Empty;
        }
    }
}
