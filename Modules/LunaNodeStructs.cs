using Newtonsoft.Json;
public class ApiRequest
{
    public string success { get; set; }
    public bool successful { get { return success == "yes"; } }
    public string error { get; set; }
}
public class VirtualMachine
{
    public string vm_id { get; set; }
    public string name { get; set; }
    public uint plan_id { get; set; }
    public string hostname { get; set; }
    // public System.Net.IPAddress primaryip { get; set; }
    public string primaryip { get; set; }
    // public System.Net.IPAddress privateip { get; set; }
    public string privateip { get; set; }
    public uint ram { get; set; }
    public uint vcpu { get; set; }
    public uint storage { get; set; }
    public uint bandwith { get; set; }
    public string region { get; set; }
    public string os_status { get; set; }
}
public class VmList : ApiRequest
{
    public VirtualMachine[] vms { get; set; }
}

public class VmInfo
{
    public VmAddress[] addresses { get; set; }
    public string status { get; set; }
    public string status_raw { get; set; }
    public string status_color { get; set; }
    public string task_state { get; set; }
    public string image { get; set; }
    public string[] security_groups { get; set; }
    public string host_id { get; set; }
    public Volume[] volumes { get; set; }
    public string status_nohtml { get; set; }
    public string os { get; set; }
    public string[] securitygroups { get { return security_groups; } set { security_groups = value; } }
    public string[] additionalip { get; set; }
    public string[] additionalprivateip { get; set; }
    public string[] ipv6 { get; set; }
    public string privateip { get; set; }
    public string ip { get; set; }
    public string login_details { get; set; }
}

public class VmAddress
{
    public string addr { get; set; }
    public uint version { get; set; }
    public string external { get; set; }
    public bool isExternal { get { return external == "1"; } }
    public string reverse { get; set; }
}

public class Volume
{
    public uint id { get; set; }
    public string name { get; set; }
}

public class VmState : ApiRequest
{
    public VmInfo info { get; set; }
    public VirtualMachine extra { get; set; }
}

internal class IPAddressConverter : JsonConverter<System.Net.IPAddress>
{
    // Not really TODO since I only plan to read IP addresses
    public override void WriteJson(JsonWriter writer, System.Net.IPAddress value, JsonSerializer serializer)
    {
        throw new System.NotImplementedException();
    }

    public override System.Net.IPAddress ReadJson(JsonReader reader, System.Type objectType, System.Net.IPAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new System.Net.IPAddress(System.Convert.ToInt64(reader.Value));
    }
}