using Newtonsoft.Json;
class LunaNodeStructs
{
    /*
    public interface ApiRequest
    {
        string success { get; }
        bool successful { get; }
        string error { get; }
    }
    */
    public struct VirtualMachine
    {
        string vm_id;
        public string name { get; }
        public uint plan_id { get; }
        string hostname;
        System.Net.IPAddress primaryip;
        System.Net.IPAddress privateip;
        uint ram;
        uint vcpu;
        uint storage;
        uint bandwith;
        string region;
        string os_status;
    }
    public struct VmList
    {
        public VirtualMachine[] vms { get; set; }
        public string success { get; }
        public bool successful { get { return success == "yes"; } }
    }
}

public class LunaNodeStructsConverter<VirtualMachine> : JsonConverter
{
    public overide void WriteJson(JsonWriter writer, VirtualMachine value, JsonSerializer serializer)
    {
        writer.WriteValueAsync("");
    }
}