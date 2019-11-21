using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS.Utils.Mongo.Model
{
    public class Attachment : MongoEntity
    {
        public string PropertyId { get; set; }
        public string ParentType { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }


        public DirectoryItem AttachmentType { get; set; }
        public DirectoryItem Visibility { get; set; }
        public string Title { get; set; }
        public DirectoryItem Category { get; set; }
        public int? TaxYear { get; set; }
        public int? TaxYearTo { get; set; }
        public int? BaseYear { get; set; }
        public float? Amount { get; set; }
        public string Description { get; set; }
        public string[] FileUrls { get; set; }

        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ClientId { get; set; }

    }

    public class DirectoryItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
