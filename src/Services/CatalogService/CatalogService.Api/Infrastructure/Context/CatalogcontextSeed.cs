using CatalogService.Api.Core.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
        {
            var policy = Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                  retryCount: 3,
                  sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                  onRetry: (exception, TimeSpan, rety, ctx) =>
                  {
                      logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {message} detexted on attempt {retry}   of {retry} ");
                  }
                );
            var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");
            var picturePath = "Pics";

            await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath, logger));

        }
        private async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath, ILogger logger)
        {
            if (!context.CatalogBrands.Any())
            {
                await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));
            }
        }

        private IEnumerator<CatalogBrand> GetCatalogBrandsFromFile(string contentPath)
        {

        }
        private IEnumerator<CatalogType> GetCatalogTypeFromFile(string contentPath)
        {
            IEnumerable<CatalogType> GetPreconfiguredTypes()
            {
                return new List<CatalogType>()
                {
                  new CatalogType{Id=1,Type="test"},
                  new CatalogType{Id=2,Type="test"},
                  new CatalogType{Id=3,Type="test"},
                  new CatalogType{Id=4,Type="test"},
                  new CatalogType{Id=5,Type="test"},
                  new CatalogType{Id=6,Type="test"},
                  new CatalogType{Id=7,Type="test"},
                  new CatalogType{Id=8,Type="test"},
                };


            }

            string fileName = Path.Combine(contentPath, "CatalogTypes.txt");

            if (!File.Exists(fileName))
            {
                return (IEnumerator<CatalogType>)GetPreconfiguredTypes();
            }

            var fileContent = File.ReadAllLines(fileName);

            var list = fileContent.Select(i => new CatalogType()
            {
                Type = i.Trim('"')
            }).Where(i => i != null);

            return (IEnumerator<CatalogType>)(list ?? GetPreconfiguredTypes());
        }
        private IEnumerator<CatalogItem> GetCatalogItemFromFile(string contentPath, CatalogContext context)
        {
            IEnumerable<CatalogItem> GetPreconfiguredItems()
            {
                return new List<CatalogItem>()
                {
                    new CatalogItem{CatalogTypeId = 2, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 1, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 3, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 4, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 5, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 6, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 7, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 8, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 9, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 10, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 11, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                    new CatalogItem{CatalogTypeId = 12, CatalogBrandId = 2, Description="asdasd",Name="test-name" },
                };


            }

            string fileName = Path.Combine(contentPath, "CatalogItems.txt");

            if (!File.Exists(fileName))
            {
                return (IEnumerator<CatalogItem>)GetPreconfiguredItems();
            }
            var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
            var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Brand, ct => ct.Id);

            var fileContent = File.ReadAllLines(fileName)
                .Skip(1) // skip header row
                .Select(i => i.Split(','))
                .Select(i => new CatalogItem()
                {
                    CatalogTypeId = catalogBrandIdLookup[i[0]],
                    CatalogBrandId = catalogTypeIdLookup[i[1]],
                    Description = i[2].Trim('"').Trim(),
                    Name = i[3].Trim('"').Trim(),
                    //Price = Decimal.Parse(i[4].Trim('"').Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                    PictureFileName = i[5].Trim('"').Trim(),
                });
            return (IEnumerator<CatalogItem>)fileContent;

        }
        private void GetCatalogItemPictures(string contentPath, string picturePath)
        {
            picturePath ??= "pics";

            if (picturePath != null)
            {
                DirectoryInfo directory = new DirectoryInfo(picturePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }
    }
}
