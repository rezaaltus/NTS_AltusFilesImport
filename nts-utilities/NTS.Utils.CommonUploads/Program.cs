using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NTS.Utils.AWS;
using NTS.Utils.BulkUploads.Properties;
using NTS.Utils.Mongo;
using System.Configuration;
using MongoDB.Driver;
using NTS.Utils.BulkUploads.Repositories;
using NTS.Utils.BulkUploads.Services;
using NTS.Utils.Mongo.Model;

namespace NTS.Utils.BulkUploads
{
    class Program
    {
        static void Main(string[] args)
        {


            string mongoDbConnection = Settings.Default.MongoDbConnection;
            string BUCKET_NAME = Settings.Default.BUCKET_NAME;
            int maxDegreeOfParallelism = Settings.Default.MaxDegreeOfParallelism;
            string S3AccessKey = Settings.Default.S3AccessKey;
            string S3SecretKey = Settings.Default.S3SecretKey;
            int range = Settings.Default.Range;
            
           
            string propertyInfoConnectionString =ConfigurationManager.ConnectionStrings["PropertyInfoConnection"].ConnectionString;
            //string fileStoreConnectionString = ConfigurationManager.ConnectionStrings["FileStoreConnection"].ConnectionString;
            //string altusfileStoreConnectionString = ConfigurationManager.ConnectionStrings["AltusFileStore1Connection"].ConnectionString;
            string altusToolsFolderName = Settings.Default.AltusToolFolderName;
            string folderNamePridict = Settings.Default.NamePredict;
          ////---------------------------------------------------------------

          //  string fileGUID = "a4d440d8-c101-48b0-8b31-b79abb119b61";
          //      byte[] data = null;
           

          //      if (fileGUID!="") // check that there is a value so it does not crash
          //      {
          //          using (var dr = SqlUtil.ExecuteReader(altusfileStoreConnectionString, "File_SelectData", "@FileGUID", fileGUID))
          //          {
          //              while (dr.Read())
          //              {
          //                  object rawData = dr["Data"];

          //                  if (rawData is byte[])
          //                      data = rawData as byte[];

          //                  break; //if there is more then one file with the same GUID then the world has probably ended!
          //              }
          //          }
          //      }
          //  FileStream fs = new FileStream(@"C:\test.pdf", FileMode.Create);
          //  fs.Write(data, 0, data.Length);
          //  fs.Close();



          //  //------------

            IMongoClient mongo = new MongoClient(mongoDbConnection);

            IDocumentUpload service = new DocumentUpload(
                    new PropertyInfoRepository(propertyInfoConnectionString, propertyInfoConnectionString,//fileStoreConnectionString, 
                    altusToolsFolderName, folderNamePridict), range,
                    maxDegreeOfParallelism, new S3Repository(S3AccessKey, S3SecretKey), 
                    BUCKET_NAME, new MongoRepositoryAttachment<Mongo.Model.Attachment>(mongo, "nts"), 
                    altusToolsFolderName, folderNamePridict );

            Stopwatch watch = new Stopwatch();
            watch.Start();
            bool result = service.Save().Result;
            watch.Stop();
            Console.WriteLine("The result was " + result);
            Console.WriteLine(watch.Elapsed);
            Console.ReadLine();


        }
        
    } //class
} // namespace 
