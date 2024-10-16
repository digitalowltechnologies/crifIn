using Crif.It.Models;
using Examine;
using Examine.Search;
using Lucene.Net.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Our.Umbraco.FullTextSearch.Interfaces;
using System.Drawing.Printing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using ContentModels = Crif.It.Models;

namespace Crif.It.Controllers
{
    public class SearchResultsController : RenderController
    {
        private readonly ISearchService _searchService;
        private readonly IExamineManager _examineManager;
        private readonly IUmbracoContextAccessor _umbracoContext;

        public SearchResultsController(ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            ISearchService searchService,
            IExamineManager examineManager) :
            base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _searchService = searchService;
            _examineManager = examineManager;
            _umbracoContext = umbracoContextAccessor;
        }

        public override IActionResult Index()
        {
            if (CurrentPage != null)
            {
                var searchModel = (ContentModels.SearchResults)CurrentPage;
                if (searchModel != null)
                {
                    string s = Request.Query["s"].ToString().ToLower();

                    /* set resultsper page to a default value if is not set in beckend searchModel.ResultsPerPage*/
                    int resultsPerPage = 5;

                    if (searchModel.ResultsPerPage > 0)
                    {
                        resultsPerPage = searchModel.ResultsPerPage;
                    }

                    BooleanQuery.MaxClauseCount = int.MaxValue;
                    
                    int.TryParse(Request.Query["page"].ToString(), out int currentPage);
                    currentPage = currentPage < 1 ? 1 : currentPage;

                    ISearchResults results;
                    if (_examineManager.TryGetIndex("ExternalIndex", out IIndex index))
                    {
                        QueryOptions queryOptions = new QueryOptions((currentPage - 1) * resultsPerPage, resultsPerPage);
                        var query = index.Searcher.CreateQuery();
                        if (!string.IsNullOrEmpty(s))
                        {
                            query = query.ManagedQuery(s).And();
                        }

                        string type = Request.Query["type"];
                        if (!string.IsNullOrEmpty(type) && (type == "articolo" || type == "articoloStorieDiSuccesso"))
                        {
                            results = query.GroupedOr(new[] { "__NodeTypeAlias" },
                                new[] { "articolo", "articoloStorieDiSuccesso" }).Execute(queryOptions);

                        }
                        else if (!string.IsNullOrEmpty(type) && type == "pagina")
                        {
                            results = query.GroupedOr(new[] { "__NodeTypeAlias" },
                             new[] { "business",  "categoriaMercato", "category", "family", "industries","industry",
                                "mercati", "mercato", "service", "services", "servizi",
                                "solutions", "soluzione", "trendTopics", "consumatori", "consumatoriCategoriaProdotti", "consumatoriProdotto", "contatti", "contatto",
                                "knowledgeEvents","singleNewsEvents","newsEvents",
                                "accademy","areaStampa","eventi", "news","newsEdEventi","ricercheAcademy","ricercheAcademyCategoria", "about", "aboutSubpage","blankPage", "home"}).Execute(queryOptions);
                        }
                        else
                        {
                            /*results = query.All().Execute(queryOptions);*/
                            results = query.GroupedOr(new[] { "__NodeTypeAlias" },
                            new[] { "articolo","articoloStorieDiSuccesso",
                                "business",  "categoriaMercato", "category", "family", "industries","industry",
                                "mercati", "mercato", "service", "services", "servizi",
                                "solutions", "soluzione", "trendTopics", "consumatori", "consumatoriCategoriaProdotti", "consumatoriProdotto", "contatti", "contatto",
                                "knowledgeEvents","singleNewsEvents","newsEvents",
                                "accademy","areaStampa","eventi", "news","newsEdEventi","ricercheAcademy","ricercheAcademyCategoria", "about", "aboutSubpage","blankPage", "home"}).Execute(queryOptions);
                        }

                        searchModel.TotalResults = (int)results.TotalItemCount;
                        searchModel.TotalPages = searchModel.TotalResults / resultsPerPage + ((searchModel.TotalResults % resultsPerPage > 0) ? 1 : 0);
                        searchModel.Results = results.Skip(0);
                    }
                }
            }
            return CurrentTemplate(CurrentPage);
        }
    }
}
