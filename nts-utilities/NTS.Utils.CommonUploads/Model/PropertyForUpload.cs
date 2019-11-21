using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS.Utils.BulkUploads.Model
{
  public  class PropertyForUpload
    {
         public int FileID { get; set; }
        public string Province { get; set; }
        public string DirectoryName { get; set; }
        public string FileName { get; set; }
        public bool IsNameCorrect { get; set; }
        public bool IsInDB { get; set; }
        public bool IsUploaded { get; set; }
        public string FileType { get; set; }
        public string MappedTo { get; set; }
        public string RollNumber { get; set; }
        public int TaxYear { get; set; }
        public string PropertyIdentificationCode { get; set; }
        public string AssessmentIdentificationCode { get; set; }

        public string ClientId { get; set; }
        public string ClientIdShort { get; set; }
    }
}
