using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS.Utils.BulkUploads.Model
{
    public class PropertyDocument
    {
        public string Name { get; set; }
        public string RollNumber { get; set; }
        public string PropertyIdentificationCode { get; set; }
        public Guid FileGuid { get; set; }
        public string ContentType { get; set; }
        public string AddressAssessorMunicipalityCode { get; set; }
    }
}
