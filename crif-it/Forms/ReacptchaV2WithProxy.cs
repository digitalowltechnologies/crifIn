using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    public class RecaptchaV2WithProxy : Umbraco.Forms.Core.Providers.FieldTypes.Recaptcha2
    {
        private readonly Recaptcha2Settings _config;
        private readonly ILogger<Recaptcha2> _logger;
        private readonly IConfiguration? _configuration;

        public RecaptchaV2WithProxy(IOptionsMonitor<Recaptcha2Settings> config, ILogger<Recaptcha2> logger, IConfiguration? configuration) : base(config, logger)
        {
            this._config = config.CurrentValue;
            this._configuration = configuration;
            this._logger = logger;
            this.Id = new Guid("B69DEAEB-ED75-4DC9-BFB8-D036BF9D3730");
            this.Name = nameof(Recaptcha2);
            this.FieldTypeViewName = "FieldType.Recaptcha2.cshtml";
            this.Description = "Google Recaptcha v2";
            this.Icon = "icon-eye";
            this.DataType = FieldDataType.String;
            this.SortOrder = 120;
            this.Category = "Simple";
        }

        public override IEnumerable<string> ValidateField(Form form, Field field, IEnumerable<object> postedValues, HttpContext context, IPlaceholderParsingService placeholderParsingService)
        {
            try
            {
                string serverAddress = "";
                string serverPort = "";
                string serverUserName = "";
                string serverPassword = "";

                if(_configuration != null)
                {
                    serverAddress  = _configuration["ProxyConfig:ServerAddress"];
                    serverPort     = _configuration["ProxyConfig:ServerPort"];
                    serverUserName = _configuration["ProxyConfig:ServerUserName"];
                    serverPassword = _configuration["ProxyConfig:ServerPassword"];
                }

                string privateKey = this._config.PrivateKey;
                if (string.IsNullOrWhiteSpace(privateKey))
                {
                    string message = "ERROR: ReCaptcha v2 is missing the Secret Key.  Please update the configuration to include a value at: " + Constants.Configuration.SectionKeys.FieldTypes.Recaptcha2 + ":PrivateKey'";
                    this._logger.LogWarning(message);
                    return (IEnumerable<string>)new string[1] { message };
                }
                StringValues stringValues = context.Request.Form["g-recaptcha-response"];
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
                interpolatedStringHandler.AppendLiteral("https://www.google.com/recaptcha/api/siteverify?secret=");
                interpolatedStringHandler.AppendFormatted(privateKey);
                interpolatedStringHandler.AppendLiteral("&response=");
                interpolatedStringHandler.AppendFormatted<StringValues>(stringValues);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                bool flag = false;
                List<string> collection1 = new List<string>();
                using (WebClient webClient = new WebClient())
                {
                    if (!string.IsNullOrEmpty(serverAddress) && !string.IsNullOrEmpty(serverPort))
                    {
                        WebProxy myproxy = new WebProxy("" + serverAddress + ":" + serverPort + "", false);
                        if (!string.IsNullOrEmpty(serverUserName) && !string.IsNullOrEmpty(serverPassword))
                        {
                            ICredentials credentials = new NetworkCredential(serverUserName, serverPassword);
                            myproxy = new WebProxy("" + serverAddress + ":" + serverPort + "", true, null, credentials);
                        }
                        webClient.Proxy = myproxy;
                    }

                    JObject jobject = JObject.Parse(webClient.DownloadString(stringAndClear));

                    if (jobject.TryGetValue("success", out JToken? jtoken1))
                    {
                        flag = jtoken1.Value<bool>();
                    }

                    if (jobject.TryGetValue("error-codes", out JToken? jtoken2))
                    {
                        IEnumerable<string> collection2 = jtoken2.Children().Select<JToken, string>((Func<JToken, string>)(x => x.Value<string>())).Where<string>((Func<string, bool>)(x => x != null)).Select<string, string>((Func<string, string>)(x => x));
                        collection1.AddRange(collection2);
                    }

                    if (flag)
                    {
                        return Enumerable.Empty<string>();
                    }

                    string str = field.Settings.ContainsKey("ErrorMessage") ? field.Settings["ErrorMessage"] : "Make sure to complete the \"I am not a robot\" challenge";
                    List<string> stringList = new List<string>();
                    stringList.Add(str);
                    stringList.AddRange((IEnumerable<string>)collection1);
                    return (IEnumerable<string>)stringList;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (IEnumerable<string>)new string[1] { e.Message };
            }
        }

    }
}