using NDIXML;
using System.ServiceModel;
namespace Models
{
    [ServiceContract]
    public interface Iconsolidataws
    {
        
        [OperationContract]
        PNCScreen HostConnect2(string s, string y,string z);
    }
}