using NDIXML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace MockNDIWCF
{
    /// <summary>
    /// Summary description for condolidata
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class condolidataws : System.Web.Services.WebService,IService1
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public PNCScreen HostConnect2(string s, string y, string z)
        {

            Console.WriteLine("Test Method Executed!");
            var sc = new PNCScreen();
            sc.Session = new Session();
            sc.Session.SessionInfo = "123";
            return sc;
        }
    }
}
