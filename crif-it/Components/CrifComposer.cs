using Crif.It.Forms;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Providers.FieldTypes;

namespace Crif.It.Components
{
    public class CrifComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.WithCollectionBuilder<WorkflowCollectionBuilder>().Add<CrifWorkflow>();

            builder.ContentFinders().Append<CrifContentFinder>();

            builder.UrlProviders().InsertBefore<DefaultUrlProvider, BusinessUrlProvider>();

            builder.WithCollectionBuilder<FieldCollectionBuilder>().Add<RadioButtonListBoxed>();
            builder.WithCollectionBuilder<FieldCollectionBuilder>().Add<RecaptchaV2WithProxy>();
        }
    }


    public class CrifContentFinder : IContentFinder
    {
        IUmbracoContextAccessor _umbracoContextAccessor;

        public CrifContentFinder(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder request)
        {
            _umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext);

            if(umbracoContext != null)
            {
                int j;
                string route;
                string folder;
                string[] segments = request.Uri.Segments;
                if(segments.Length > 2 && segments[1].Length == 3)
                {
                    folder = segments[2];
                    j = 3;
                }
                else
                {
                    folder = segments[1];
                    j = 2;
                }

                if (folder.Contains("services") || folder.Contains("industries"))
                {
                    route = segments[0] + "business/" + folder;

                    for (int i = j; i < segments.Length; i++)
                    {
                        route += segments[i];
                    }

                    if (!route.EndsWith("/")) route += "/";

                    IPublishedContent? content = umbracoContext?.Content?.GetByRoute(route);
                    request.SetPublishedContent(content);
                    return Task.FromResult(true);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }
            else
            {
                return Task.FromResult(false);
            }

            throw new NotImplementedException();
        }
    }

    public class BusinessUrlProvider : DefaultUrlProvider
    {
        public BusinessUrlProvider(IOptionsMonitor<RequestHandlerSettings> requestSettings, ILogger<DefaultUrlProvider> logger, ISiteDomainMapper siteDomainMapper, IUmbracoContextAccessor umbracoContextAccessor, UriUtility uriUtility) : base(requestSettings, logger, siteDomainMapper, umbracoContextAccessor, uriUtility)
        {
        }

        public override IEnumerable<UrlInfo> GetOtherUrls(int id, Uri current)
        {
            // Add custom logic to return 'additional urls' - this method populates a list of additional urls for the node to display in the Umbraco backoffice
            return base.GetOtherUrls(id, current);
        }

        public override UrlInfo? GetUrl(IPublishedContent content, UrlMode mode, string? culture, Uri current)
        {
            if (content is null)
            {
                return null;
            }

            // Only apply this to product pages
            UrlInfo? defaultUrlInfo = base.GetUrl(content, mode, culture, current);
            if (defaultUrlInfo is null)
            {
                return null;
            }

            if (!defaultUrlInfo.IsUrl)
            {
                // This is a message (eg published but not visible because the parent is unpublished or similar)
                return defaultUrlInfo;
            }
            else if(defaultUrlInfo.Text.Contains("business/") && content.ContentType.Alias != "business")
            {
                // Manipulate the url somehow in a custom fashion:
                //var newUrl = defaultUrlInfo.Text.Replace("business/", "");
                    
                //return new UrlInfo(newUrl, true, defaultUrlInfo.Culture);
            }

            // Otherwise return the base GetUrl result:
            return base.GetUrl(content, mode, culture, current);
        }

    }


}
