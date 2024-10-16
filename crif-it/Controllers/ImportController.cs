using Crif.It.Utils;
using System.Globalization;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using System.Text;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.Extensions.Logging;
using static Lucene.Net.Util.Fst.Util;

namespace Crif.It.Controllers
{
    class Article
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string BodyText { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string PublicationDate { get; set; } = "";
        public string ActualDate { get; set; } = "";
        public string BrowserTitle { get; set; } = "";
        public string MetaDescription { get; set; } = "";
        public string SocialMediaTitle { get; set; } = "";
        public string SocialMediaImage { get; set; } = "";
        public string SocialMediaDescription { get; set; } = "";
        public string UrlName { get; set; } = "";
        public string AttachmentUrl { get; set; } = "";
        public string ThumbnailImage { get; set; } = "";
    }

    public class ImportController : UmbracoAuthorizedApiController
    {
        private readonly IContentService _contentService;
        private readonly ILogger<ImportController> _logger;
        private readonly HttpClient _httpClient;

        private readonly MediaFileManager _mediaFileManager;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
        private readonly MediaUrlGeneratorCollection _mediaUrlGeneratorCollection;
        private readonly IMediaService _mediaService;
        private readonly IConfiguration _configuration;
        private readonly IUmbracoContextAccessor _accessor;

        public ImportController(
            IContentService contentService,
            MediaFileManager mediaFileManager,
            IShortStringHelper shortStringHelper,
            IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
            MediaUrlGeneratorCollection mediaUrlGeneratorCollection,
            IMediaService mediaService,
            IConfiguration configuration,
            IUmbracoContextAccessor accessor,
            ILogger<ImportController> logger)
        {
            _logger = logger;
            _mediaService = mediaService;
            _contentService = contentService;
            _mediaFileManager = mediaFileManager;
            _shortStringHelper = shortStringHelper;
            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
            _mediaUrlGeneratorCollection = mediaUrlGeneratorCollection;
            _configuration = configuration;
            _accessor = accessor;

            _httpClient = new HttpClient();
        }

        // ~/umbraco/backoffice/api/import/news
        public async Task<string> News()
        {
            _accessor.TryGetUmbracoContext(out var context);
            IPublishedContent? root = context?.Content?
                .GetAtRoot()?.First()?
                .Children(x => x.ContentType.Alias == "knowledgeEvents")?.First()?
                .Children(x => x.ContentType.Alias == "newsEvents")?
                .Where(x => x.Name == "News & Events").First();

            if(root == null)
            {
                return "job failed, missing root folder";
            }
            
            return await ImportContent(root, "news.csv");
        }

        // ~/umbraco/backoffice/api/import/events
        public async Task<string> Events()
        {
            _accessor.TryGetUmbracoContext(out var context);
            IPublishedContent? root = context?.Content?
                .GetAtRoot()?.First()?
                .Children(x => x.ContentType.Alias == "knowledgeEvents")?.First()?
                .Children(x => x.ContentType.Alias == "newsEvents")?
                .Where(x => x.Name == "News & Events").First();

            if (root == null)
            {
                return "job failed, missing root folder";
            }

            return await ImportContent(root, "events.csv");
        }

        // ~/umbraco/backoffice/api/import/pressreleases
        public async Task<string> PressReleases()
        {
            _accessor.TryGetUmbracoContext(out var context);
            IPublishedContent? root = context?.Content?
                .GetAtRoot()?.First()?
                .Children(x => x.ContentType.Alias == "knowledgeEvents")?.First()?
                .Children(x => x.ContentType.Alias == "newsEvents")?
                .Where(x => x.Name == "Press").First();

            if (root == null)
            {
                return "job failed, missing root folder";
            }

            return await ImportContent(root, "press_releases.csv");
        }

        private async Task<string> ImportContent(IPublishedContent startNode, string filename)
        {
            int succeed = 0;
            int failed = 0;

            string csvPath = _configuration["CSVPath"];

            List<Article> articles = new List<Article>();

            using (var reader = new StreamReader(csvPath + filename))
            {
                bool first = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (first)
                    {
                        first = false;
                    }
                    else if (line != null)
                    {
                        int i = 0;
                        var values = line.Split(';');
                        Article article = new Article();

                        article.Id = values[i++];
                        article.Name = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.Title = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.BodyText = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.ShortDescription = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 100);
                        article.PublicationDate = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.ActualDate = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.BrowserTitle = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.MetaDescription = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.SocialMediaTitle = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.SocialMediaImage = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.SocialMediaDescription = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.UrlName = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));

                        articles.Add(article);
                    }
                }
            }

            foreach (var article in articles)
            {
                try
                {
                    IContent newContent = _contentService.Create(article.Name, startNode.Key, "singleNewsEvents");
                    newContent.SetCultureName(article.Name, "en-GB");
                    newContent.SetValue("singleNewsEventsTitle", article.Title, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsBody", article.BodyText, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsAbstract", article.ShortDescription, "en-GB");

                    DateTime dateTime = DateTime.Now;
                    try
                    {
                        dateTime = DateTime.ParseExact(article.PublicationDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        // dont' care
                    }
                    newContent.PublishDate = dateTime;
                    newContent.SetValue("customDate", dateTime, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsDateAndLocation", article.ActualDate, "en-GB");
                    newContent.SetValue("title", article.BrowserTitle, "en-GB");
                    newContent.SetValue("metaDescription", article.MetaDescription, "en-GB");
                    newContent.SetValue("openGraphTitle", article.SocialMediaTitle, "en-GB");

                    var defaultImage = _mediaService.GetById(Guid.Parse("ada3b95d-8a0d-49f8-828d-331b63f241a9"));
                    if (defaultImage != null)
                    {
                        newContent.SetValue("singleKnowledgeEventsCoverImage", defaultImage.GetUdi().ToString());
                    }

                    var result = await TryDownloadMedia(article.SocialMediaImage, "Imported", Constants.Conventions.MediaTypes.Image);

                    newContent.SetValue("openGraphImage", result);
                    newContent.SetValue("openGraphDescription", article.SocialMediaDescription, "en-GB");
                    newContent.SetValue("umbracoUrlName", article.UrlName, "en-GB");

                    _contentService.SaveAndPublish(newContent);

                    succeed++;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed import of article ({0}), title {1}", article.Id, article.Name);

                    failed++;
                }

            }

            return "job done. Succeeded " + succeed + ", Failed " + failed;
        }



        public async Task<string> Press()
        {
            int succeed = 0;
            int failed = 0;

            List<Article> articles = new List<Article>();

            string csvPath = _configuration["CSVPath"];

            _accessor.TryGetUmbracoContext(out var context);
            IPublishedContent? root = context?.Content?
                .GetAtRoot()?.First()?
                .Children(x => x.ContentType.Alias == "knowledgeEvents")?.First()?
                .Children(x => x.ContentType.Alias == "newsEvents")?
                .Where(x => x.Name == "Press review").First();

            if (root == null)
            {
                return "job failed, no parent directory";
            }
            

            using (var reader = new StreamReader(csvPath + "press.csv"))
            {
                bool first = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (first)
                    {
                        first = false;
                    }
                    else if (line != null)
                    {
                        int i = 0;
                        var values = line.Split(';');
                        Article article = new Article();
                        article.Id = values[i++];
                        article.Name = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.Title = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.ShortDescription = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 100);
                        article.PublicationDate = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.ActualDate = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.AttachmentUrl = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));

                        articles.Add(article);
                    }
                }
            }

            foreach (var article in articles)
            {
                try
                {
                    IContent newContent = _contentService.Create(article.Name, root.Key, "singleNewsEvents");
                    newContent.SetCultureName(article.Name, "en-GB");
                    newContent.SetValue("singleNewsEventsTitle", article.Title, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsAbstract", article.ShortDescription, "en-GB");

                    DateTime dateTime = DateTime.Now;
                    try
                    {
                        dateTime = DateTime.ParseExact(article.PublicationDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        // dont' care
                    }
                    newContent.PublishDate = dateTime;
                    newContent.SetValue("customDate", dateTime, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsDateAndLocation", article.ActualDate, "en-GB");

                    var defaultImage = _mediaService.GetById(Guid.Parse("ada3b95d-8a0d-49f8-828d-331b63f241a9"));
                    if (defaultImage != null)
                    {
                        newContent.SetValue("singleKnowledgeEventsCoverImage", defaultImage.GetUdi().ToString());
                    }

                    var result = await TryDownloadMedia(article.AttachmentUrl, "Attachments", Constants.Conventions.MediaTypes.File);

                    if(!string.IsNullOrEmpty(result))
                    {
                        newContent.SetValue("attachmentLink", result, "en-GB");
                        newContent.SetValue("isAPressRelease", true);
                    }

                    _contentService.SaveAndPublish(newContent);

                    succeed++;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed import of article ({0}), title {1}", article.Id, article.Name);

                    failed++;
                }

            }

            return "job done. Succeeded " + succeed + ", Failed " + failed;
        }


        // ~/umbraco/api/import/news
        public async Task<string> Research()
        {
            int succeed = 0;
            int failed = 0;

            List<Article> articles = new List<Article>();

            string csvPath = _configuration["CSVPath"];

            _accessor.TryGetUmbracoContext(out var context);
            IPublishedContent? root = context?.Content?
                .GetAtRoot()?.First()?
                .Children(x => x.ContentType.Alias == "knowledgeEvents")?.First()?
                .Children(x => x.ContentType.Alias == "newsEvents")?
                .Where(x => x.Name == "Resources").First();

            if (root == null)
            {
                return "job failed, missing root folder";
            }

            using (var reader = new StreamReader(csvPath + "research.csv"))
            {
                bool first = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (first)
                    {
                        first = false;
                    }
                    else if (line != null)
                    {
                        int i = 0;
                        var values = line.Split(';');
                        Article article = new Article();

                        article.Id = values[i++];
                        article.Name = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.Title = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.ThumbnailImage = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.BodyText = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.ShortDescription = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 100);
                        article.PublicationDate = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.ActualDate = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.BrowserTitle = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.MetaDescription = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.SocialMediaTitle = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.SocialMediaImage = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));
                        article.SocialMediaDescription = HtmlUtils.TextCut(Encoding.UTF8.GetString(Convert.FromBase64String(values[i++])), 512);
                        article.UrlName = Encoding.UTF8.GetString(Convert.FromBase64String(values[i++]));

                        articles.Add(article);
                    }
                }
            }

            foreach (var article in articles)
            {
                try
                {
                    IContent newContent = _contentService.Create(article.Name, root.Key, "singleNewsEvents");
                    newContent.SetCultureName(article.Name, "en-GB");
                    newContent.SetValue("singleNewsEventsTitle", article.Title, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsBody", article.BodyText, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsAbstract", article.ShortDescription, "en-GB");

                    DateTime dateTime = DateTime.Now;
                    try
                    {
                        dateTime = DateTime.ParseExact(article.PublicationDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        // dont' care
                    }
                    newContent.PublishDate = dateTime;
                    newContent.SetValue("customDate", dateTime, "en-GB");
                    newContent.SetValue("singleKnowledgeEventsDateAndLocation", article.ActualDate, "en-GB");
                    newContent.SetValue("title", article.BrowserTitle, "en-GB");
                    newContent.SetValue("metaDescription", article.MetaDescription, "en-GB");
                    newContent.SetValue("openGraphTitle", article.SocialMediaTitle, "en-GB");

                    var defaultImage = _mediaService.GetById(Guid.Parse("ada3b95d-8a0d-49f8-828d-331b63f241a9"));
                    if (defaultImage != null)
                    {
                        newContent.SetValue("singleKnowledgeEventsCoverImage", defaultImage.GetUdi().ToString());
                    }

                    if (!string.IsNullOrEmpty(article.ThumbnailImage)) 
                    {
                        var result = await TryDownloadMedia(article.ThumbnailImage, "Imported", Constants.Conventions.MediaTypes.Image);
                        newContent.SetValue("singleKnowledgeEventsCoverImage", result);
                    }

                    if (!string.IsNullOrEmpty(article.SocialMediaImage))
                    { 
                        var result = await TryDownloadMedia(article.SocialMediaImage, "Imported", Constants.Conventions.MediaTypes.Image);
                        newContent.SetValue("openGraphImage", result);
                    }

                    newContent.SetValue("openGraphDescription", article.SocialMediaDescription, "en-GB");
                    newContent.SetValue("umbracoUrlName", article.UrlName, "en-GB");

                    _contentService.SaveAndPublish(newContent);

                    succeed++;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed import of article ({0}), title {1}", article.Id, article.Name);

                    failed++;
                }

            }

            return "job done. Succeeded " + succeed + ", Failed " + failed;
        }

        private async Task<string> TryDownloadMedia(string url, string folderName, string mediaType)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string filename = (url.Split("/").Last()) ?? "defaultname";
                string imagename = filename.Split(".").FirstOrDefault() ?? "defaultname.png";

                if (!url.StartsWith("http"))
                {
                    url = "https://www.crif.com" + url;
                }

                var folder = _mediaService.GetRootMedia().FirstOrDefault(m => m.Name.InvariantEquals(folderName));
                if (folder == null)
                {
                    folder = _mediaService.CreateMedia(folderName, Constants.System.Root, Constants.Conventions.MediaTypes.Folder);

                    _mediaService.Save(folder);
                }

                byte[] fileBytes = await _httpClient.GetByteArrayAsync(url);

                using (Stream stream = new MemoryStream(fileBytes))
                {
                    var media = _mediaService.CreateMedia(imagename, folder, mediaType);
                    media.SetValue(_mediaFileManager, _mediaUrlGeneratorCollection, _shortStringHelper, _contentTypeBaseServiceProvider, Constants.Conventions.Media.File, filename, stream);

                    var result = _mediaService.Save(media);

                    return media.GetUdi().ToString();
                }
            }

            return "";
        }



    }
}
