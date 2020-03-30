using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VollyV3.Services.ImageManager
{
    public class ImageManagerImpl : IImageManager
    {
        private static readonly string StorageName = Environment.GetEnvironmentVariable("storage_name");
        private static readonly string StorageApiKey = Environment.GetEnvironmentVariable("storage_api");
        private static readonly string ImageContainer = Environment.GetEnvironmentVariable("images_container");
        private const int ImageResizeWidth = 1024;
        private const int ImageResizeHeight = 768;

        public async Task<string> UploadImageAsync(IFormFile imageFile, string imageName)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    StorageName, StorageApiKey), true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(ImageContainer);
            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(imageName.Replace(" ", String.Empty));

            await blob.DeleteIfExistsAsync();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var image = Image.FromStream(imageFile.OpenReadStream(), true, true))
                {
                    using var newImage = new Bitmap(ImageResizeWidth, ImageResizeHeight);
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        var adjustedHeight = ImageResizeHeight;
                        var adjustedWidth = ImageResizeWidth;

                        if (image.Height > image.Width)
                        {
                            adjustedWidth = (int)Math.Round(ImageResizeWidth * ((double)ImageResizeHeight / image.Height));
                        }
                        else
                        {
                            adjustedHeight = (int)Math.Round(ImageResizeHeight * ((double)ImageResizeWidth / image.Width));
                        }

                        graphics.DrawImage(
                            image,
                            (ImageResizeWidth - adjustedWidth) / 2, (ImageResizeHeight - adjustedHeight) / 2,
                            adjustedWidth, adjustedHeight
                            );
                    }
                    newImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                }

                CloudBlobStream blobStream = blob.OpenWriteAsync().Result;

                memoryStream.Position = 0;

                await memoryStream.CopyToAsync(blobStream);

                await blobStream.CommitAsync();
            }

            return blob.Uri.ToString();
        }
    }
}
