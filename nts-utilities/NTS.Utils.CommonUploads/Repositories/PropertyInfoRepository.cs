using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using NTS.Utils.BulkUploads.Model;

namespace NTS.Utils.BulkUploads.Repositories
{


    public interface IPropertyInfoRepository
    {
        Task<IEnumerable<PropertyDocument>> GetAllPropertyInfo(string altusToolFolderName, string namePredict);

        Task<IEnumerable<PropertyForUpload>> GetAtlanticPropertyInfo(string altusToolFolderName, string namePredict);

        Task<bool> UpdateUploadStatus(int noteID);
        Task<bool> UpdateUploadFilesToBeLoadedStatus(int noteID);
        Task<IEnumerable<PropertyForUploadBC>> GetBCPropertyInfo(string altusToolFolderName, string namePredict);
        Task<IEnumerable<ContractsForUploadON>> GetONContractsInfo(string altusToolFolderName, string namePredict);
        Task<IEnumerable<NotesForUpload>> GetNotesInfo(string altusToolFolderName, string namePredict);
        Task<IEnumerable<FilesTobeLoadedForUpload>> GetFilesToBeLoadedInfo(string altusToolFolderName, string namePredict);
    }


    class PropertyInfoRepository : IPropertyInfoRepository
    {

        private readonly string propertyConnectionString;
        private readonly string fileStorageConnectionString;

        public PropertyInfoRepository(string propertyConnectionString, string fileStorageConnectionString, string altusToolFolderName, string namePredict)
        {
            this.propertyConnectionString = propertyConnectionString;
            this.fileStorageConnectionString = fileStorageConnectionString;




        }



        public async Task<bool> UpdateUploadStatus(int fileID)
        {
            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);

                string command = "UPDATE [NationalTax_WorkingTemp].[dbo].[ON_Files] SET IsUploaded =1 WHERE FileID = " + fileID ;
               // string command = "UPDATE [NTS_Ontario_Migration].[dbo].[NotesLoaded_Status] SET IsUploaded =1 WHERE noteID = " + noteID;
                SqlCommand cmd = new SqlCommand(command, cnn);
                  cmd.ExecuteNonQuery();
            }

            return true;

        }
        //UpdateUploadFilesToBeLoadedStatus
        public async Task<bool> UpdateUploadFilesToBeLoadedStatus(int fID)
        {
            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);

                //string command = "UPDATE AtlanticFiles_PROD SET IsUploaded =1 WHERE FileID = " + fileID ;
                string command = "UPDATE [NTS_Ontario_Migration].[dbo].[FilesToBeLoaded_Status] SET IsUploaded =1 WHERE FId = " + fID;
                SqlCommand cmd = new SqlCommand(command, cnn);
                cmd.ExecuteNonQuery();
            }

            return true;

        }

        public async Task<IEnumerable<PropertyForUpload>> GetAtlanticPropertyInfo(string altusToolFolderName, string namePredict)
        {
            //string command = @"SELECT * FROM AtlanticFiles_DEV WHERE FileID IN (34615,
            //                                34616,
            //                                34617,
            //                                34618,
            //                                34619)";




            // string command = @"SELECT * FROM AtlanticFiles_QA WHERE FileID IN (69798,38075,38104)";


            string command = @"SELECT  * ,
                                        '' AS ClientId ,
                                        '' AS ClientIdShort
                                FROM    dbo.AtlanticFiles_PROD
                                WHERE   MappedTo = 'Assessment'
                                        AND AssessmentIdentificationCode IS NOT NULL
                                        AND FileType NOT IN ( 'Annual RFI Submission',
                                                              'Request for Information', 
                                                              'Confirmation Notice',
                                                              'Amended Notice' )
                                        AND IsUploaded IS NULL
                                        --AND FileID IN (50637,50655,50656,50660,50661)   ";
             


            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);
                SqlCommand cmd = new SqlCommand(command, cnn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                List< PropertyForUpload> propList = new List<PropertyForUpload>();

                while (reader.Read())
                {
                    var property = new PropertyForUpload
                    {
                        FileID = Convert.ToInt32(reader["FileID"]),
                        FileName = reader["FileName"]?.ToString() ?? "",
                        DirectoryName = reader["DirectoryName"].ToString(),
                        PropertyIdentificationCode = reader["PropertyIdentificationCode"].ToString(),
                        Province = reader["Province"].ToString(),
                        IsNameCorrect = !(reader["IsNameCorrect"] is DBNull) && Convert.ToBoolean(reader["IsNameCorrect"]),
                        IsInDB =  !(reader["IsInDB"] is    DBNull) && Convert.ToBoolean(reader["IsInDB"]),
                        IsUploaded = !(reader["IsUploaded"] is DBNull) && Convert.ToBoolean(reader["IsUploaded"]),
                        FileType = reader["FileType"].ToString(),
                        MappedTo = reader["MappedTo"].ToString(),
                        RollNumber = reader["RollNumber"].ToString(),
                        TaxYear = reader["TaxYear"] is DBNull ? 0 : Convert.ToInt32(reader["TaxYear"]),
                        AssessmentIdentificationCode = reader["AssessmentIdentificationCode"] is DBNull ? "" : 
                                                           reader["AssessmentIdentificationCode"].ToString(),
                         ClientIdShort = reader["ClientIdShort"].ToString(),
                         ClientId = reader["ClientId"].ToString(),

                    };


                    propList.Add(property);

                }

                // return reader.HasRows ? Mapper.Map<IEnumerable<PropertyForUpload>>(reader) : new List<PropertyForUpload>();
                return propList;
            }



        }
// ------------Reza Jan 10 2018

        public async Task<IEnumerable<PropertyForUploadBC>> GetBCPropertyInfo(string altusToolFolderName, string namePredict)
        {
           
            string command = @"Select * from BC_FilesAllInOne  order by FileID asc"; 
           // string command = @"Select   * from BC_FilesAllInOne_AmountNULL order by FileID asc";-- WHERE Amount IS NOT null



            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);
                SqlCommand cmd = new SqlCommand(command, cnn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                List<PropertyForUploadBC> propList = new List<PropertyForUploadBC>();

                while (reader.Read())
                {
                    var property = new PropertyForUploadBC
                    {
                        FileID =  Convert.ToInt32(reader["FileID"]),
                        FileName = reader["FileName"]?.ToString() ?? "",
                        DirectoryName = reader["DirectoryName"].ToString(),
                        PropertyIdentificationCode = reader["PropertyIdentificationCode"].ToString(),
                        Province =reader["Province"].ToString(),
                        FileType = reader["FileType"].ToString(),
                        RollNumber = reader["RollNumber"].ToString(),
                        MappedTo = reader["MappedTo"].ToString(),
                        PropertyName= reader["PropertyName"].ToString(),
                        TaxYear =reader["TaxYear"] is DBNull ? 0 : Convert.ToInt32(reader["TaxYear"]),
                        Amount = (reader["Amount"] is DBNull ? 0 : Convert.ToDecimal(reader["Amount"]))

                    };


                    propList.Add(property);

                }

                // return reader.HasRows ? Mapper.Map<IEnumerable<PropertyForUpload>>(reader) : new List<PropertyForUpload>();
                return propList;
            }



        }

        //-----------------------------
        //-----Reza Aug 31-------------
        public async Task<IEnumerable<ContractsForUploadON>> GetONContractsInfo(string altusToolFolderName, string namePredict)
        {

            string command = @"Select  [FileID],[FileName],[DirectoryName],[SFContractID],[Province],[FileType],[DeltekNumber],[TaxYear],[MappedTo],[ClientName],'' as ClientId from ON_FilesAllInOne
 where FileID>5 order by FileID asc";
            // string command = @"Select   * from BC_FilesAllInOne_AmountNULL order by FileID asc";-- WHERE Amount IS NOT null



            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);
                SqlCommand cmd = new SqlCommand(command, cnn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                List<ContractsForUploadON> contractList = new List<ContractsForUploadON>();

                while (reader.Read())
                {
                    var contract = new ContractsForUploadON
                    {
                        FileID = Convert.ToInt32(reader["FileID"]),
                        FileName = reader["FileName"]?.ToString() ?? "",
                        DirectoryName = reader["DirectoryName"].ToString(),
                        SFContractID = reader["SFContractID"].ToString(),
                        Province = reader["Province"].ToString(),
                        FileType = reader["FileType"].ToString(),
                        DeltekNumber  = reader["DeltekNumber"].ToString(),
                        MappedTo = reader["MappedTo"].ToString(),
                        ClientId = reader["ClientId"].ToString(),
                        TaxYear = reader["TaxYear"] is DBNull || reader["TaxYear"] == "" ? 0  : Convert.ToInt32(reader["TaxYear"]),
                       

                    };


                    contractList.Add(contract);

                }

                // return reader.HasRows ? Mapper.Map<IEnumerable<PropertyForUpload>>(reader) : new List<PropertyForUpload>();
                return contractList;
            }



        }

        //------ Reza Sep 17 2018

        public async Task<IEnumerable<NotesForUpload>> GetNotesInfo(string altusToolFolderName, string namePredict)
        {

            string command = @"Select  * from [NTS_Ontario_Migration].[dbo].[NotesTobeLoaded]   order by NoteID asc";
            // string command = @"Select   * from BC_FilesAllInOne_AmountNULL order by FileID asc";-- WHERE Amount IS NOT null



            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);
                SqlCommand cmd = new SqlCommand(command, cnn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                List<NotesForUpload> noteList = new List<NotesForUpload>();

                while (reader.Read())
                {
                    var note = new NotesForUpload
                    {
                        NoteID = Convert.ToInt32(reader["NoteID"]),
                        PropertyId = reader["PropertyId"]?.ToString() ?? "",
                        ParentType = reader["ParentType"].ToString(),
                        ParentId = reader["ParentId"].ToString(),
                        ParentName = reader["ParentName"].ToString(),
                        AttachmentTypeCode = reader["AttachmentTypeCode"].ToString(),
                        AttachmentTypeName = reader["AttachmentTypeName"].ToString(),
                        VisibilityCode = reader["VisibilityCode"].ToString(),
                        VisibilityName = reader["VisibilityName"].ToString(),
                        Title = reader["Title"].ToString(),
                        CategoryCode = reader["CategoryCode"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        Description = reader["Description"].ToString(),
                        CreatedById = reader["CreatedById"].ToString(),
                        CreatedByName = reader["CreatedByName"].ToString(),
                        CreatedOn = (reader["CreatedOn"] == null) ? null : (reader["CreatedOn"].ToString()),
                        ModifiedById = reader["ModifiedById"].ToString(),
                        ModifiedByName = reader["ModifiedByName"].ToString(),
                        ModifiedOn = (reader["ModifiedOn"]==null) ? null: reader["ModifiedOn"].ToString() ,
                        ClientId = reader["ClientId"].ToString()
                        //TaxYear = reader["TaxYear"] is DBNull ? 0 : Convert.ToInt32(reader["TaxYear"]),


                    };


                    noteList.Add(note);

                }

                // return reader.HasRows ? Mapper.Map<IEnumerable<PropertyForUpload>>(reader) : new List<PropertyForUpload>();
                return noteList;
            }



        }
        public async Task<IEnumerable<FilesTobeLoadedForUpload>> GetFilesToBeLoadedInfo(string altusToolFolderName, string namePredict)
        {

            string command = @"Select  * from [NTS_Ontario_Migration].[dbo].[FilesTobeLoadedToMongo]   order by FId asc";
            // string command = @"Select   * from BC_FilesAllInOne_AmountNULL order by FileID asc";-- WHERE Amount IS NOT null



            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);
                SqlCommand cmd = new SqlCommand(command, cnn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                List<FilesTobeLoadedForUpload> fileList = new List<FilesTobeLoadedForUpload>();

                while (reader.Read())
                {
                    var file = new FilesTobeLoadedForUpload
                    {
                        FId = Convert.ToInt32(reader["FId"]),
                        PropertyId = reader["PropertyId"]?.ToString() ?? "",
                        FileGuid = reader["FileGUID"].ToString(),
                        ParentType = reader["ParentType"].ToString(),
                        ParentId = reader["ParentId"].ToString(),
                        ParentName = reader["ParentName"].ToString(),
                        AttachmentTypeCode = reader["AttachmentTypeCode"].ToString(),
                        AttachmentTypeName = reader["AttachmentTypeName"].ToString(),
                        VisibilityCode = reader["VisibilityCode"].ToString(),
                        VisibilityName = reader["VisibilityName"].ToString(),
                        Title = reader["Title"].ToString(),
                        FileType = reader["ContentType"].ToString(),
                        FileName = reader["FileName"].ToString(),
                        CategoryCode = reader["CategoryCode"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        Description = reader["Description"].ToString(),
                        CreatedById = reader["CreatedById"].ToString(),
                        CreatedByName = reader["CreatedByName"].ToString(),
                        CreatedOn = (reader["CreatedOn"] == null) ? null : (reader["CreatedOn"].ToString()),
                        ModifiedById = reader["ModifiedById"].ToString(),
                        ModifiedByName = reader["ModifiedByName"].ToString(),
                        ModifiedOn = (reader["ModifiedOn"] == null) ? null : reader["ModifiedOn"].ToString(),
                        ClientId = reader["ClientId"].ToString(),
                       // TaxYear = reader["TaxYear"] is DBNull ? 0 : Convert.ToInt32(reader["TaxYear"]),
                        TaxYear = (reader["TaxYear"] == null) ?null: reader["TaxYear"].ToString(),
                        TaxYearTo = (reader["TaxYearTo"] == null) ? null : reader["TaxYearTo"].ToString()

                    };


                    fileList.Add(file);

                }

                // return reader.HasRows ? Mapper.Map<IEnumerable<PropertyForUpload>>(reader) : new List<PropertyForUpload>();
                return fileList;
            }



        }

        public async Task<IEnumerable<PropertyDocument>> GetAllPropertyInfo(string altusToolFolderName, string namePredict)
        {
            string command = "SELECT f.Name,p.RollNumber,dp.PropertyIdentificationCode,f.FileGUID,f.ContentType,dp.AddressAssessorMunicipalityCode FROM NationalTax_WorkingTemp_NoDeply.dbo.PropertyInfo dp "
                             + "INNER JOIN Yeoman.dbo.Properties p ON dp.RegionalPropertyCode = p.RollNumber "
                             + "INNER JOIN Yeoman.dbo.ProjectTax pt ON pt.PropID = p.PropID AND pt.Status <> 0 "
                             + "INNER JOIN NationalTax_WorkingTemp_NoDeply.dbo.Folders fld  ON fld.ParentGUID = pt.ProjectGUID AND fld.Name = 'Assessment' "
                             + "INNER JOIN NationalTax_WorkingTemp_NoDeply.dbo.Folders fld2 ON fld.FolderGUID = fld2.ParentGUID AND fld2.name = '" + altusToolFolderName + "' "
                             + "INNER JOIN NationalTax_WorkingTemp_NoDeply.dbo.Files f ON f.ParentGUID = fld2.folderguid "
                             + "WHERE f.Name LIKE '%" + namePredict + "%' ";

            using (SqlConnection cnn = new SqlConnection(propertyConnectionString))
            {
                await cnn.OpenAsync().ConfigureAwait(false);
                SqlCommand cmd = new SqlCommand(command, cnn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                return reader.HasRows ? Mapper.Map<IEnumerable<PropertyDocument>>(reader) : new List<PropertyDocument>();
            }



        }






    }
}
