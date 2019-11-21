using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTS.Utils.BulkUploads.Model
{
  public  class NotesForUpload
    {
        public int NoteID { get; set; }
        public string PropertyId { get; set; }
        public string ParentType { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string ClientId { get; set; }
        public string AttachmentTypeCode { get; set; }
        public string AttachmentTypeName { get; set; }
        public string VisibilityCode { get; set; }
        public string VisibilityName { get; set; }
        public string Title { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
       
        public string Description { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }

        public string CreatedOn { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedOn { get; set; }
       
       
       

    }
}
