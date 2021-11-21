using Microsoft.AspNetCore.Mvc.Filters;
using Models;
using NDIXML;
using System;
using System.Threading;
using System.Xml.Linq;



public class consolidataws : Iconsolidataws
{

    private ThreadLocal<string> _paramValue = new ThreadLocal<string>() { Value = string.Empty };
    public void SetParameterForSomeOperation(string paramValue)
    {
        _paramValue.Value = paramValue;
    }

    public string SomeOperationName()
    {
        return "Param value from http header: " + _paramValue.Value;
    }

    public PNCScreen HostConnect2(string s,string y,string z)
    {

        Console.WriteLine("Test Method Executed!");
        var sc = new PNCScreen();
        sc.Session = new Session();
        sc.Session.SessionInfo = "123";
        return sc;
    }

}