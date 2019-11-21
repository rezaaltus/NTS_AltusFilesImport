using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace NTS.Utils.AWS
{
    public interface IS3Repository
    {
        Task<string> SaveFileToBucket(string bucketName, string extension, string contentType, byte[] imageBytes, bool isPublic);
    }

    public class S3Repository : IS3Repository
    {

        private readonly string _AwsAccessKeyId;
        private readonly string _AwsSecretAccessKey;


        public S3Repository(string awsAccessKeyId, string awsSecretAccessKey)
        {
            _AwsAccessKeyId = awsAccessKeyId;
            _AwsSecretAccessKey = awsSecretAccessKey;
        }

        public async Task<string> SaveFileToBucket(string bucketName, string extension, string contentType, byte[] fileBytes, bool isPublic)
        {

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    await memStream.WriteAsync(fileBytes, 0, fileBytes.Length);

                    string fileName = string.Format("{0}.{1}", Guid.NewGuid(), extension);

                    using (TransferUtility fileTransferUtility = new TransferUtility(_AwsAccessKeyId, _AwsSecretAccessKey, RegionEndpoint.USEast1))
                    {
                        TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = bucketName,
                            StorageClass = S3StorageClass.Standard,
                            Key = fileName,
                            CannedACL = isPublic ? S3CannedACL.PublicRead : S3CannedACL.BucketOwnerFullControl,
                           // ContentType = contentType,
                            InputStream = memStream,
                        };

                        await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                        return fileName;
                    }
                }


            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception);
                return "";
            }

        }
    }
}
