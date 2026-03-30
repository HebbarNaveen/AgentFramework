using System;
using System.Collections.Generic;


namespace CustomAgent.Models;

// configuration models
public class Configuration
{
    public McpClients mcp_servers { get; set; }
    public List<RestApi> rest_apis { get; set; }
    public Model model  { get; set; }
}

public class McpClients
{
    public List<LocalServer> local_servers { get; set; }
    public List<RemoteServer> remote_servers { get; set; }
}

public class LocalServer
{
    public string name { get; set; }
    public string command { get; set; }
    public List<string> args { get; set; }
    public Dictionary<string, string> environment_variables { get; set; }
}

public class RemoteServer
{
    public string name { get; set; }
    public string url { get; set; }
    public string api_key { get; set; }
}

public class RestApi
{
    public string plugin_name { get; set; }
    public string uri { get; set; }
}

public class Model
{
    public string id { get; set; }
    public string endpoint { get; set; }
    public string api_key { get; set; }
}


