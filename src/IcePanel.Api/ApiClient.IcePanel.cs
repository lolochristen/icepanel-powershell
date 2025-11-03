using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace IcePanel.Api;
public partial class ApiClient
{
    public ApiClient(string apikey) : this(new HttpClientRequestAdapter(new ApiKeyAuthenticationProvider("ApiKey "+apikey,
        "Authorization", ApiKeyAuthenticationProvider.KeyLocation.Header)))
    {
    }
}

