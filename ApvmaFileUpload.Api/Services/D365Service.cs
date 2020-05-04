using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using ApvmaFileUpload.Api.Models;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ApvmaFileUpload.Api.Services
{
    public class D365Service : ID365Service
    {
        private CrmServiceClient _svcClient;
        
        public D365Service()
        {
            _svcClient = new CrmServiceClient(ConfigurationManager.ConnectionStrings["apvma-crm"].ToString());
        }
        public Contact GetTokenById(string id)
        {
            Contact contact = new Contact();
            if (_svcClient != null && _svcClient.IsReady)
            {
                // Retrieve the several attributes from the new account.
                ColumnSet cols = new ColumnSet(
                    new String[] { "fullname", "firstname", "lastname" });
                Entity retrievedContact =  _svcClient.Retrieve("contact", new Guid(id), cols);
                if (retrievedContact != null)
                {
                    //token = CreateToken();
                    contact = PopulateToken(retrievedContact);
                }
            }
            return contact;
        }

        private Contact PopulateToken(Entity entity)
        {
            if (entity == null || entity.Attributes == null) return null;
            return new Contact
            {
                AzureUrl = ConfigurationManager.AppSettings["AzureUrl"],
                FirstName = entity.Contains("firstname") ? entity.GetAttributeValue<string>("firstname") : string.Empty,
                LastName = entity.Contains("lastname") ? entity.GetAttributeValue<string>("lastname") : string.Empty,
                Token = CreateToken()
            };
        }

        private string CreateToken()
        {
            string sasToken = string.Empty;
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                //create cloudblobclient that represents the blob storrage endpoint for storage account
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                //get container called apvma-largeblob
                cloudBlobContainer = cloudBlobClient.GetContainerReference("apvma-largeblob");

                var writeOnlyPolicy = new SharedAccessBlobPolicy()
                {
                    SharedAccessStartTime = DateTime.UtcNow,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(10),
                    Permissions = SharedAccessBlobPermissions.Read  |
                                  SharedAccessBlobPermissions.Write |
                                  SharedAccessBlobPermissions.List
                };

                sasToken = cloudBlobContainer.GetSharedAccessSignature(writeOnlyPolicy);
            }
            return sasToken;
        }
    }
}