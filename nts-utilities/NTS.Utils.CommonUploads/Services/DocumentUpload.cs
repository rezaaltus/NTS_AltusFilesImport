using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTS.Utils.AWS;
using NTS.Utils.BulkUploads.Model;
using NTS.Utils.BulkUploads.Repositories;
using NTS.Utils.Mongo;
using NTS.Utils.Mongo.Model;
using Attachment = System.Net.Mail.Attachment;

namespace NTS.Utils.BulkUploads.Services
{


    public interface IDocumentUpload
    {
        Task<bool> Save();
    }


  public  class DocumentUpload : IDocumentUpload
    {


        private readonly int _range;
        private readonly int _maxDegreeOfParallelism;
        private readonly IPropertyInfoRepository _propertyInfoRepository;
        private readonly IS3Repository _s3Repository;
        private readonly string _bucketName;
        private readonly MongoRepositoryAttachment<Mongo.Model.Attachment> _mongoRepository;
        //string altusfileStoreConnectionString = ConfigurationManager.ConnectionStrings["AltusFileStore1Connection"].ConnectionString;
        //   private readonly MongoRepositoryBase<ParentAlternateInfo> _mongoRepositoryParent;

        private readonly string altusToolsFolderName;
        private readonly string folderNamePridict;

        public DocumentUpload(IPropertyInfoRepository propertyInfoRepository, int range, 
            int maxDegreeOfParallelism, IS3Repository s3Repository, 
            string bucketName, MongoRepositoryAttachment< Mongo.Model.Attachment> mongoRepository, 
            string altusToolsFolderName, string folderNamePridict  )
        {
            _propertyInfoRepository = propertyInfoRepository;
            _range = range;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _s3Repository = s3Repository;
            _bucketName = bucketName;
            _mongoRepository = mongoRepository;
            this.altusToolsFolderName = altusToolsFolderName;
            this.folderNamePridict = folderNamePridict;
          //  _mongoRepositoryParent = mongoRepositoryParent;
        }



        public  Mongo.Model.Attachment  GetMongoRecord(PropertyForUpload property)
        {

            Mongo.Model.Attachment attachment =  new Mongo.Model.Attachment();
            attachment.FileUrls = new[] { property.FileName  };
            attachment.TaxYear = property.TaxYear;
            attachment.PropertyId = property.PropertyIdentificationCode;
            attachment.CreatedOn =    DateTime.Now;
            attachment.CreatedById = "Bulk Upload Service";
            attachment.CreatedByName = "Bulk Upload Service";
            attachment.AttachmentType = new DirectoryItem { Code = "2", Name = "Attachment" };
           
            
            attachment.Description = "";
            attachment.Visibility = new DirectoryItem
            {
                Code = "1",
                Name = "Internal"
            };
            attachment.ModifiedById = "Bulk Upload Service";
            attachment.ModifiedByName = "Bulk Upload Service";
            attachment.ModifiedOn = DateTime.Now;
            attachment.Title = property.FileName;
            switch (property.MappedTo)
            {
                case "Property":
                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "property" };
                        attachment.ParentType = "assessorDocument";
                        attachment.ParentId = property.PropertyIdentificationCode;
                        attachment.ParentName = "";
                        break;
                    }
                case "Assessment":
                {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "assessment" };
                        attachment.ParentType = "assessment";
                        attachment.ParentId = property.AssessmentIdentificationCode;
                        attachment.ParentName = "Assessment";
                        break;
                }
                case "Appeal Detail":
                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "assessment" };
                        attachment.ParentType = "assessment";
                        attachment.ParentId = property.AssessmentIdentificationCode;
                        attachment.ParentName = "Assessment";
                        break;
                    }

                case "General Details":
                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "GENERAL_DETAIL" };
                        attachment.ParentType = "GENERAL_DETAIL";
                        attachment.ParentId = property.PropertyIdentificationCode;
                        attachment.ParentName = "General Details";
                        break;
                    }

                case "Bill Slips":
                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "assessment" };
                        attachment.ParentType = "assessment";
                        attachment.ParentId = property.AssessmentIdentificationCode;
                        attachment.ParentName = "Assessment";
                        break;
                    }
                case "Client" :
                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "SF_CLIENT" };
                        attachment.ParentType = "SF_CLIENT";
                        attachment.ParentId = property.ClientIdShort;
                        
                        attachment.ParentName = "Clients";
                        attachment.ClientId = property.ClientId;
                        attachment.PropertyId = property.ClientIdShort;
                        break;
                    }
                default:

                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "GENERAL_DETAIL" };
                        attachment.ParentType = "GENERAL_DETAIL";
                        attachment.ParentId = property.PropertyIdentificationCode;
                        attachment.ParentName = "General Details";
                        break;
                    }
            }
            return attachment;
        }

        public Mongo.Model.Attachment GetMongoRecordBC(PropertyForUploadBC property)
        {

            Mongo.Model.Attachment attachment = new Mongo.Model.Attachment();
            attachment.FileUrls = new[] { property.FileName };
            attachment.TaxYear = property.TaxYear;
            attachment.PropertyId = property.PropertyIdentificationCode;
            attachment.CreatedOn = DateTime.Now;
            attachment.CreatedById = "Bulk Upload Service";
            attachment.CreatedByName = "Bulk Upload Service";
            attachment.AttachmentType = new DirectoryItem { Code = "assessorDocument", Name = "Assessor Document" };

            attachment.Description = "";
            attachment.Visibility = new DirectoryItem
            {
                Code = "1",
                Name = "Internal"
            };
            attachment.ModifiedById = "Bulk Upload Service";
            attachment.ModifiedByName = "Bulk Upload Service";
            attachment.ModifiedOn = DateTime.Now;
            attachment.Title = property.FileName;
            switch (property.MappedTo)
            {
                case "Property":
                    {
                        attachment.Category = new DirectoryItem { Code = "ROLLRETURN", Name = "Assessment Roll Return" };
                        attachment.ParentType = "assessorDocument";
                        attachment.ParentId = property.PropertyIdentificationCode;
                        attachment.ParentName = property.PropertyName;
                        attachment.Amount =(float) property.Amount;

                        break;
                    }
                
                default:

                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "GENERAL_DETAIL" };
                        attachment.ParentType = "GENERAL_DETAIL";
                        attachment.ParentId = property.PropertyIdentificationCode;
                        attachment.ParentName = "General Details";
                        break;
                    }
            }
            return attachment;
        }
        public Mongo.Model.Attachment GetMongoRecordON(ContractsForUploadON contract)
        {

            Mongo.Model.Attachment attachment = new Mongo.Model.Attachment();
            attachment.FileUrls = new[] { contract.FileName };
            attachment.TaxYear =contract.TaxYear;
            attachment.PropertyId = contract.SFContractID;
            attachment.CreatedOn = DateTime.Now;
            attachment.CreatedById = "Bulk Upload Service";
            attachment.CreatedByName = "Bulk Upload Service";
            attachment.AttachmentType = new DirectoryItem { Code = "2", Name = "Attachment" };

            attachment.Description = "";
            attachment.Visibility = new DirectoryItem
            {
                Code = "1",
                Name = "Internal"
            };
            attachment.ModifiedById = "Bulk Upload Service";
            attachment.ModifiedByName = "Bulk Upload Service";
            attachment.ModifiedOn = DateTime.Now;
            attachment.Title = contract.FileName.Replace(".pdf", "");

            switch (contract.MappedTo)
            {
                case "Contract":
                    {
                        attachment.Category = new DirectoryItem { Code = "1", Name = "SF_CONTRACT" };
                        attachment.ParentType = "SF_CONTRACT";
                        attachment.ParentId =contract.SFContractID;
                        attachment.ParentName = "Contracts";
                        attachment.ClientId = null;
                       

                        break;
                    }

               
            }
            return attachment;
        }
        public Mongo.Model.Attachment GetMongoRecordNote(NotesForUpload note)
        {

            Mongo.Model.Attachment attachment = new Mongo.Model.Attachment();
         //   attachment.FileUrls = new[] { "" };
            attachment.TaxYear =null;//note.TaxYear;
            attachment.PropertyId = note.PropertyId;
            if (note.CreatedOn == "")
            {
                attachment.CreatedOn = null;
            }
            else
            {
                attachment.CreatedOn = Convert.ToDateTime(note.CreatedOn);
            }
             
            attachment.CreatedById = note.CreatedById;
            attachment.CreatedByName = note.CreatedByName;
            attachment.AttachmentType = new DirectoryItem { Code =note.AttachmentTypeCode, Name = note.AttachmentTypeName };

            attachment.Description = note.Description;
            attachment.Visibility = new DirectoryItem
            {
                Code = note.VisibilityCode,
                Name = note.VisibilityName
            };
            attachment.ModifiedById = note.ModifiedById;
            attachment.ModifiedByName = note.ModifiedByName;
            if (note.ModifiedOn == "")
            {
                attachment.ModifiedOn = null;
            }
            else
            {
                attachment.ModifiedOn =Convert.ToDateTime(note.ModifiedOn);
            }
            
            attachment.Title = note.Title;

            //switch (note.MappedTo)
            //{
            //    case "Note":
            //        {
                        attachment.Category = new DirectoryItem { Code = note.CategoryCode, Name = note.CategoryName };
                        attachment.ParentType = note.ParentType;
                        attachment.ParentId = note.ParentId;
                        attachment.ParentName = note.ParentName;
                        attachment.ClientId = note.ClientId;


                    //    break;
                    //}


            //}
            return attachment;
        }
        public Mongo.Model.Attachment GetMongoRecordFilesTobeLoaded(FilesTobeLoadedForUpload file)
        {

            Mongo.Model.Attachment attachment = new Mongo.Model.Attachment();
            //   attachment.FileUrls = new[] { "" };
            if (file.TaxYear == "0")
            {
                attachment.TaxYear = null;
            }
            else
            {
                attachment.TaxYear = Convert.ToInt32(file.TaxYear);
            }
            if (file.TaxYearTo == "0")
            {
                attachment.TaxYearTo = null;
            }
            else
            {
                attachment.TaxYearTo = Convert.ToInt32(file.TaxYearTo);
            }
            
            // null;//note.TaxYear;
            attachment.PropertyId = file.PropertyId;
            if (file.CreatedOn == "")
            {
                attachment.CreatedOn = null;
            }
            else
            {
                attachment.CreatedOn = Convert.ToDateTime(file.CreatedOn);
            }

            attachment.CreatedById = file.CreatedById;
            attachment.CreatedByName = file.CreatedByName;
            attachment.AttachmentType = new DirectoryItem { Code = file.AttachmentTypeCode, Name = file.AttachmentTypeName };

            attachment.Description = file.Description;
            attachment.Visibility = new DirectoryItem
            {
                Code = file.VisibilityCode,
                Name = file.VisibilityName
            };
            attachment.ModifiedById = file.ModifiedById;
            attachment.ModifiedByName = file.ModifiedByName;
            if (file.ModifiedOn == "")
            {
                attachment.ModifiedOn = null;
            }
            else
            {
                attachment.ModifiedOn = Convert.ToDateTime(file.ModifiedOn);
            }

            attachment.Title = file.Title;

            //switch (note.MappedTo)
            //{
            //    case "Note":
            //        {
            attachment.Category = new DirectoryItem { Code = file.CategoryCode, Name = file.CategoryName };
            attachment.ParentType = file.ParentType;
            attachment.ParentId = file.ParentId;
            attachment.ParentName = file.ParentName;
            attachment.ClientId = file.ClientId;


            //    break;
            //}


            //}
            return attachment;
        }
        public async Task<bool> Save()
        {
            //List<PropertyForUpload> propertInfos = (await _propertyInfoRepository.GetAtlanticPropertyInfo(altusToolsFolderName, folderNamePridict).ConfigureAwait(false)).ToList();
            //List<PropertyForUploadBC> propertInfos = (await _propertyInfoRepository.GetBCPropertyInfo(altusToolsFolderName, folderNamePridict).ConfigureAwait(false)).ToList();
            List<ContractsForUploadON> contractInfos = (await _propertyInfoRepository.GetONContractsInfo(altusToolsFolderName, folderNamePridict).ConfigureAwait(false)).ToList();
            //List<NotesForUpload> noteInfos = (await _propertyInfoRepository.GetNotesInfo(altusToolsFolderName, folderNamePridict).ConfigureAwait(false)).ToList();
           // List<FilesTobeLoadedForUpload> fileToBeLoadedInfos = (await _propertyInfoRepository.GetFilesToBeLoadedInfo(altusToolsFolderName, folderNamePridict).ConfigureAwait(false)).ToList();
            if (!contractInfos.Any()) return false;
            Console.WriteLine("{0} files ready to upload", contractInfos.Count());
            OrderablePartitioner<Tuple<int, int>> partitioner = Partitioner.Create(0, contractInfos.Count(), _range);
            Parallel.ForEach(partitioner, new ParallelOptions { MaxDegreeOfParallelism = _maxDegreeOfParallelism }, range =>
            {
               // List<FilesTobeLoadedForUpload> fileInfosChunk = new List<FilesTobeLoadedForUpload>();
                List<ContractsForUploadON> fileInfosChunk = new List<ContractsForUploadON>();
                //List<PropertyForUpload> propertyInfosChunk = new List<PropertyForUpload>();
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    fileInfosChunk.Add(contractInfos[i]);
                }
                foreach (var item in fileInfosChunk)
                {
                    string path = "";
                    try
                    {
                        //---------------------------------------------------------------

                        //string fileGUID = item.FileGuid;//"a4d440d8-c101-48b0-8b31-b79abb119b61";
                        //byte[] data = null;


                        //if (fileGUID != "") // check that there is a value so it does not crash
                        //{
                        //    using (var dr = SqlUtil.ExecuteReader(altusfileStoreConnectionString, "File_SelectData", "@FileGUID", fileGUID))
                        //    {
                        //        while (dr.Read())
                        //        {
                        //            object rawData = dr["Data"];

                        //            if (rawData is byte[])
                        //                data = rawData as byte[];

                        //            break; //if there is more then one file with the same GUID then the world has probably ended!
                        //        }
                        //    }
                        //}
                        ////FileStream fs = new FileStream(@"C:\test.pdf", FileMode.Create);
                        ////fs.Write(data, 0, data.Length);
                        ////fs.Close();

                        //----------------------------- Make sure file name is unique------------------------

                        //------------
                        int? year = item.TaxYear;
                        path = "C:\\ONFiles2019" + "\\" + item.FileName;//+ "";
                        var fs = File.ReadAllBytes(path);
                        string fileName = _s3Repository.SaveFileToBucket(_bucketName,
                                                        // Path.GetExtension("").Replace(".", string.Empty),
                                                     Path.GetExtension(item.FileName).Replace(".", string.Empty),
                                                          "", fs, false).Result;
                        //string fileName = _s3Repository.SaveFileToBucket(_bucketName,
                        //    Path.GetExtension(item.FileName).Replace(".", string.Empty),
                        //    "", data, false).Result;
                        // var attachment = GetMongoRecord(item);
                        // var attachment = GetMongoRecordBC(item);
                       // var attachment = GetMongoRecordFilesTobeLoaded(item);
                          var attachment = GetMongoRecordON(item);
                        // var attachment = GetMongoRecordNote(item);
                        attachment.FileUrls = new[] { fileName };
                        string documentId = _mongoRepository.UpsertAttachment(attachment).Result;

                      //  _propertyInfoRepository.UpdateUploadFilesToBeLoadedStatus(item.FileID);
                        _propertyInfoRepository.UpdateUploadStatus(item.FileID);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(item.FileID + " Uploaded! " + documentId);
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("ERR_FILE " +  path);
                      //  throw;
                    }

                    
                   
                }
            });
            return true;
        }

    }
}
