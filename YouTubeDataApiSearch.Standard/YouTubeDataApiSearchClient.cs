// <copyright file="YouTubeDataApiSearchClient.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace YouTubeDataApiSearch.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using YouTubeDataApiSearch.Standard.Authentication;
    using YouTubeDataApiSearch.Standard.Controllers;
    using YouTubeDataApiSearch.Standard.Http.Client;
    using YouTubeDataApiSearch.Standard.Utilities;

    /// <summary>
    /// The gateway for the SDK. This class acts as a factory for Controller and
    /// holds the configuration of the SDK.
    /// </summary>
    public sealed class YouTubeDataApiSearchClient : IConfiguration
    {
        // A map of environments and their corresponding servers/baseurls
        private static readonly Dictionary<Environment, Dictionary<Server, string>> EnvironmentsMap =
            new Dictionary<Environment, Dictionary<Server, string>>
        {
            {
                Environment.Production, new Dictionary<Server, string>
                {
                    { Server.Default, "https://www.googleapis.com/youtube/v3" },
                }
            },
        };

        private readonly IDictionary<string, IAuthManager> authManagers;
        private readonly IHttpClient httpClient;

        private readonly Lazy<SimpleSearchController> simpleSearch;

        private YouTubeDataApiSearchClient(
            Environment environment,
            IDictionary<string, IAuthManager> authManagers,
            IHttpClient httpClient,
            IHttpClientConfiguration httpClientConfiguration)
        {
            this.Environment = environment;
            this.httpClient = httpClient;
            this.authManagers = (authManagers == null) ? new Dictionary<string, IAuthManager>() : new Dictionary<string, IAuthManager>(authManagers);
            this.HttpClientConfiguration = httpClientConfiguration;

            this.simpleSearch = new Lazy<SimpleSearchController>(
                () => new SimpleSearchController(this, this.httpClient, this.authManagers));
        }

        /// <summary>
        /// Gets SimpleSearchController controller.
        /// </summary>
        public SimpleSearchController SimpleSearchController => this.simpleSearch.Value;

        /// <summary>
        /// Gets the configuration of the Http Client associated with this client.
        /// </summary>
        public IHttpClientConfiguration HttpClientConfiguration { get; }

        /// <summary>
        /// Gets Environment.
        /// Current API environment.
        /// </summary>
        public Environment Environment { get; }

        /// <summary>
        /// Gets auth managers.
        /// </summary>
        internal IDictionary<string, IAuthManager> AuthManagers => this.authManagers;

        /// <summary>
        /// Gets http client.
        /// </summary>
        internal IHttpClient HttpClient => this.httpClient;

        /// <summary>
        /// Gets the URL for a particular alias in the current environment and appends
        /// it with template parameters.
        /// </summary>
        /// <param name="alias">Default value:DEFAULT.</param>
        /// <returns>Returns the baseurl.</returns>
        public string GetBaseUri(Server alias = Server.Default)
        {
            StringBuilder url = new StringBuilder(EnvironmentsMap[this.Environment][alias]);
            ApiHelper.AppendUrlWithTemplateParameters(url, this.GetBaseUriParameters());

            return url.ToString();
        }

        /// <summary>
        /// Creates an object of the YouTubeDataApiSearchClient using the values provided for the builder.
        /// </summary>
        /// <returns>Builder.</returns>
        public Builder ToBuilder()
        {
            Builder builder = new Builder()
                .Environment(this.Environment)
                .HttpClient(this.httpClient)
                .AuthManagers(this.authManagers)
                .HttpClientConfig(config => config.Build());

            return builder;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                $"Environment = {this.Environment}, " +
                $"HttpClientConfiguration = {this.HttpClientConfiguration}, ";
        }

        /// <summary>
        /// Creates the client using builder.
        /// </summary>
        /// <returns> YouTubeDataApiSearchClient.</returns>
        internal static YouTubeDataApiSearchClient CreateFromEnvironment()
        {
            var builder = new Builder();

            string environment = System.Environment.GetEnvironmentVariable("YOU_TUBE_DATA_API_SEARCH_STANDARD_ENVIRONMENT");

            if (environment != null)
            {
                builder.Environment(ApiHelper.JsonDeserialize<Environment>($"\"{environment}\""));
            }

            return builder.Build();
        }

        /// <summary>
        /// Makes a list of the BaseURL parameters.
        /// </summary>
        /// <returns>Returns the parameters list.</returns>
        private List<KeyValuePair<string, object>> GetBaseUriParameters()
        {
            List<KeyValuePair<string, object>> kvpList = new List<KeyValuePair<string, object>>()
            {
            };
            return kvpList;
        }

        /// <summary>
        /// Builder class.
        /// </summary>
        public class Builder
        {
            private Environment environment = YouTubeDataApiSearch.Standard.Environment.Production;
            private IDictionary<string, IAuthManager> authManagers = new Dictionary<string, IAuthManager>();
            private HttpClientConfiguration.Builder httpClientConfig = new HttpClientConfiguration.Builder();
            private IHttpClient httpClient;

            /// <summary>
            /// Sets Environment.
            /// </summary>
            /// <param name="environment"> Environment. </param>
            /// <returns> Builder. </returns>
            public Builder Environment(Environment environment)
            {
                this.environment = environment;
                return this;
            }

            /// <summary>
            /// Sets HttpClientConfig.
            /// </summary>
            /// <param name="action"> Action. </param>
            /// <returns>Builder.</returns>
            public Builder HttpClientConfig(Action<HttpClientConfiguration.Builder> action)
            {
                if (action is null)
                {
                    throw new ArgumentNullException(nameof(action));
                }

                action(this.httpClientConfig);
                return this;
            }

            /// <summary>
            /// Sets the IHttpClient for the Builder.
            /// </summary>
            /// <param name="httpClient"> http client. </param>
            /// <returns>Builder.</returns>
            internal Builder HttpClient(IHttpClient httpClient)
            {
                this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
                return this;
            }

            /// <summary>
            /// Sets the authentication managers for the Builder.
            /// </summary>
            /// <param name="authManagers"> auth managers. </param>
            /// <returns>Builder.</returns>
            internal Builder AuthManagers(IDictionary<string, IAuthManager> authManagers)
            {
                this.authManagers = authManagers ?? throw new ArgumentNullException(nameof(authManagers));
                return this;
            }

            /// <summary>
            /// Creates an object of the YouTubeDataApiSearchClient using the values provided for the builder.
            /// </summary>
            /// <returns>YouTubeDataApiSearchClient.</returns>
            public YouTubeDataApiSearchClient Build()
            {
                this.httpClient = new HttpClientWrapper(this.httpClientConfig.Build());

                return new YouTubeDataApiSearchClient(
                    this.environment,
                    this.authManagers,
                    this.httpClient,
                    this.httpClientConfig.Build());
            }
        }
    }
}
