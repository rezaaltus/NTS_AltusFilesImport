using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS.Utils.BulkUploads.Model
{
  public  class ContractsForUploadON
    {
        public int FileID { get; set; }
        public string Province { get; set; }
        public string DirectoryName { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string DeltekNumber { get; set; }
        public int? TaxYear { get; set; }
        public string SFContractID { get; set; }
        public string MappedTo { get; set; }
        public string ClientId { get; set; }

    }
}
