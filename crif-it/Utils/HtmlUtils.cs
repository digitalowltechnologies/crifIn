using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Umbraco.Cms.Core.Models;

namespace Crif.It.Utils
{
    public class HtmlUtils
    {
        static public string TextCut(string? textIn, int chars = 125)
        {
            string ret = "";

            if(textIn != null)
            {
                ret = textIn;

                if (textIn.Length > chars)
                {
                    ret = textIn.Substring(0, chars - 3);
                    ret += "...";
                }
            }
            
            return ret;
        }
        static public string FormatUrl(string url, bool forceHttps = true)
        {
            string ret = "";
            if(forceHttps)
            {
                ret = url.ReplaceFirst("http://", "https://");
            }

            if (!ret.EndsWith("/"))
                ret += "/";

            return ret;
        }

        static public string FormatUrl(HttpRequest? request, bool forceHttps = true)
        {
            string ret = "";
            
            if(request != null)
            {
                ret = request.Scheme;
                if (forceHttps && ret == "http") ret = "https";
                ret += "://" + request.Host;
                ret += request.Path;
            }

            if (!ret.EndsWith("/"))
                ret += "/";

            return ret;
        }

        static public string GetImageAlt(MediaWithCrops? media)
        {
            string? ret = media?.Name;

            if(media?.HasValue("alt") == true)
            {
                ret = (string?)media?.GetProperty("alt")?.GetValue();
            }

            return ret ?? string.Empty;
        }

        static public string GetCategory(string alias)
        {
            string cat;
            switch(alias)
            {
                case "consumatoriProdotto":
                    cat = "Prodotto Consumatori";
                    break;
                case "servizi":
                    cat = "Servizi";
                    break;
                case "soluzioni":
                    cat = "Soluzioni";
                    break;
                case "mercato":
                    cat = "Mercato";
                    break;
                case "articoloStorieDiSuccesso":
                    cat = "Storia di successo";
                break;
                case "contatti":
                case "contatto":
                    cat = "Contatti";
                    break;
                case "articolo":
                    cat = "Articolo";
                    break;
                case "academy":
                    cat = "Academy";
                    break;
                case "areaStampa":
                    cat = "Area Stampa";
                    break;
                case "eventi":
                    cat = "Eventi";
                    break;
                case "news":
                    cat = "News";
                    break;
                case "ricerche":
                    cat = "Ricerche";
                    break;
                case "about":
                case "blankPage":
                case "aboutSubpage":
                    cat = "Pagina";
                    break;
                default:
                    cat = "Pagina";
                break;
            }

            return cat;
        }

        static public string HighlightText(string? word, string? text)
        {
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(word))
            {
                return text.Replace(word, string.Format("<strong>{0}</strong>", word));
            }

            return "";
        }
    }
}
