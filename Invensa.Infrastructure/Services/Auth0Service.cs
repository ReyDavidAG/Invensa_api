namespace Invensa.Infrastructure.Services;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Custom;
using Domain.Entities;
using Interfaces;
using Invensa.Domain.Exceptions;
using Invensa.Domain.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

public class Auth0Service : IAuth0Service
{
    private readonly Auth0Configuration _auth0Config;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<Auth0Service> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public Auth0Service(IOptions<Auth0Configuration> auth0Config, ILogger<Auth0Service> logger,
        IHttpClientFactory clientFactory, IUnitOfWork unitOfWork)
    {
        _auth0Config = auth0Config.Value;
        _logger = logger;
        _clientFactory = clientFactory;
        _unitOfWork = unitOfWork;
    }

  
}