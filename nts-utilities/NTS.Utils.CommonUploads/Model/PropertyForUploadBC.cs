using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS.Utils.BulkUploads.Model
{
  public  class PropertyForUploadBC
    {
        public int FileID { get; set; }
        public string Province { get; set; }
        public string DirectoryName { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string RollNumber { get; set; }
        public int TaxYear { get; set; }
        public string PropertyIdentificationCode { get; set; }
        public string MappedTo { get; set; }
        public string PropertyName { get; set; }
        public decimal Amount { get; set; }
    }
}
