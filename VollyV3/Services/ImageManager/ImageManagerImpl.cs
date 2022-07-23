using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace VollyV3.Services.ImageManager
{
    public class ImageManagerImpl : IImageManager
    {
        private static readonly string StorageName = Environment.GetEnvironmentVariable("storage_name");
        private static readonly string StorageApiKey = Environment.GetEnvironmentVariable("storage_api_key");
        private static readonly string ImageContainer = Environment.GetEnvironmentVariable("images_container");
        private const int ImageFixedWidth = 1024;
        private const int ImageFixedHeight = 768;
        private const double ImageAspectRatio = 4.0 / 3.0;

        public async Task<string> UploadOpportunityImageAsync(Stream imageStream, string imageName)
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
                using (var image = Image.FromStream(imageStream, true, true))
                {
                    var imageWidth = ImageFixedWidth;
                    var imageHeight = ImageFixedHeight;

                    var widthOffset = 0;
                    var heightOffset = 0;

                    if ((double)image.Width / image.Height > ImageAspectRatio)
                    {
                        imageHeight = (int)Math.Round(image.Height * (double)ImageFixedWidth / image.Width);
                        heightOffset = (int)Math.Abs((ImageFixedHeight - imageHeight) / 2);
                    }
                    else
                    {
                        imageWidth = (int)Math.Round(image.Width * (double)ImageFixedHeight / image.Height);
                        widthOffset = (int)Math.Abs((ImageFixedWidth - imageWidth) / 2);
                    }

                    using var newImage = new Bitmap(ImageFixedWidth, ImageFixedHeight);
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.DrawImage(
                            image,
                            widthOffset, heightOffset,
                            imageWidth, imageHeight
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
