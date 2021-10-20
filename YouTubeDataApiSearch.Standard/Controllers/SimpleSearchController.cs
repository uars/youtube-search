// <copyright file="SimpleSearchController.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace YouTubeDataApiSearch.Standard.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Converters;
    using YouTubeDataApiSearch.Standard;
    using YouTubeDataApiSearch.Standard.Authentication;
    using YouTubeDataApiSearch.Standard.Exceptions;
    using YouTubeDataApiSearch.Standard.Http.Client;
    using YouTubeDataApiSearch.Standard.Http.Request;
    using YouTubeDataApiSearch.Standard.Http.Response;
    using YouTubeDataApiSearch.Standard.Utilities;

    /// <summary>
    /// SimpleSearchController.
    /// </summary>
    public class SimpleSearchController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSearchController"/> class.
        /// </summary>
        /// <param name="config"> config instance. </param>
        /// <param name="httpClient"> httpClient. </param>
        /// <param name="authManagers"> authManager. </param>
        internal SimpleSearchController(IConfiguration config, IHttpClient httpClient, IDictionary<string, IAuthManager> authManagers)
            : base(config, httpClient, authManagers)
        {
        }

        /// <summary>
        /// Search by Keyword EndPoint.
        /// </summary>
        /// <param name="search">Required parameter: Example: search.</param>
        /// <param name="part">Required parameter: Example: snippet.</param>
        /// <param name="maxResults">Required parameter: Example: 25.</param>
        /// <param name="key">Required parameter: API-Key.</param>
        /// <param name="q">Required parameter: keyword.</param>
        /// <returns>Returns the dynamic response from the API call.</returns>
        public dynamic SearchByKeyword(
                Models.TypeSearchEnum search,
                string part,
                int maxResults,
                string key,
                string q)
        {
            Task<dynamic> t = this.SearchByKeywordAsync(search, part, maxResults, key, q);
            ApiHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Search by Keyword EndPoint.
        /// </summary>
        /// <param name="search">Required parameter: Example: search.</param>
        /// <param name="part">Required parameter: Example: snippet.</param>
        /// <param name="maxResults">Required parameter: Example: 25.</param>
        /// <param name="key">Required parameter: API-Key.</param>
        /// <param name="q">Required parameter: keyword.</param>
        /// <param name="cancellationToken"> cancellationToken. </param>
        /// <returns>Returns the dynamic response from the API call.</returns>
        public async Task<dynamic> SearchByKeywordAsync(
                Models.TypeSearchEnum search,
                string part,
                int maxResults,
                string key,
                string q,
                CancellationToken cancellationToken = default)
        {
            // the base uri for api requests.
            string baseUri = this.Config.GetBaseUri();

            // prepare query string for API call.
            StringBuilder queryBuilder = new StringBuilder(baseUri);
            queryBuilder.Append("/{search}");

            // process optional template parameters.
            ApiHelper.AppendUrlWithTemplateParameters(queryBuilder, new Dictionary<string, object>()
            {
                { "search", ApiHelper.JsonSerialize(search).Trim('\"') },
            });

            // prepare specfied query parameters.
            var queryParams = new Dictionary<string, object>()
            {
                { "part", part },
                { "maxResults", maxResults },
                { "key", key },
                { "q", q },
            };

            // append request with appropriate headers and parameters
            var headers = new Dictionary<string, string>()
            {
                { "user-agent", this.UserAgent },
                { "accept", "application/json" },
            };

            // prepare the API call request to fetch the response.
            HttpRequest httpRequest = this.GetClientInstance().Get(queryBuilder.ToString(), headers, queryParameters: queryParams);

            // invoke request and get response.
            HttpStringResponse response = await this.GetClientInstance().ExecuteAsStringAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            HttpContext context = new HttpContext(httpRequest, response);

            if (response.StatusCode == 400)
            {
                throw new InvalidArgumentException("The parameter specified has an invalid argument", context);
            }

            if (response.StatusCode == 402)
            {
                throw new InvalidVideoIdException("The relatedToVideo parameter specified an invalid video ID", context);
            }

            // handle errors defined at the API level.
            this.ValidateResponse(response, context);

            return ApiHelper.JsonDeserialize<dynamic>(response.Body);
        }
    }
}