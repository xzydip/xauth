using System;
using X_Auth;

class Program
{
    // XZY AUTH style — name / ownerid / secret / version, no API key.
    // Get Owner ID + Secret from: panel -> Install page.
    private static readonly api XzyAuthApp = new api(
        name:    "APP-1LHEK3",
        ownerid: "IHH231792GMAILCOM-OFPDR",
        secret:  "SEC_fV0a-6poU-U6sp-xmAN-Wimi-j3ZF-D2HE-d5Hh",
        version: "1.0"
    );

    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--full")
        {
            FullControl();
            return;
        }

        if (args.Length > 0 && args[0] == "--license")
        {
            string lk = args.Length > 1 ? args[1] : "";
            Console.WriteLine("Server : " + api.Server);
            Console.WriteLine("App    : " + XzyAuthApp.name + " v" + XzyAuthApp.version);
            Console.Write("init() ");
            XzyAuthApp.init();
            Console.WriteLine(XzyAuthApp.response.success ? "OK" : "FAIL: " + XzyAuthApp.response.message);
            Console.Write("license(" + lk + ") ");
            XzyAuthApp.license(lk);
            Console.WriteLine(XzyAuthApp.response.success
                ? "=> LOGIN OK @ " + XzyAuthApp.response.expiry
                : "=> DENIED: " + XzyAuthApp.response.message);
            return;
        }

        string user = args.Length > 0 ? args[0] : "username";
        string pass = args.Length > 1 ? args[1] : "password";

        Console.WriteLine("Server : " + api.Server);
        Console.WriteLine("App    : " + XzyAuthApp.name + " v" + XzyAuthApp.version);

        Console.Write("init() ");
        XzyAuthApp.init();
        Console.WriteLine(XzyAuthApp.response.success ? "OK" : "FAIL: " + XzyAuthApp.response.message);

        Console.Write("login(" + user + ") ");
        XzyAuthApp.login(user, pass);
        Console.WriteLine(XzyAuthApp.response.success
            ? "=> LOGIN OK @ " + XzyAuthApp.response.expiry
            : "=> DENIED: " + XzyAuthApp.response.message);

        return;
    }

    // Full control demo: create -> login -> ban -> unban -> login -> delete
    static void FullControl()
    {
        XzyAuthApp.init();
        Console.WriteLine("Server : " + api.Server);
        Console.WriteLine("App    : " + XzyAuthApp.name + " (ownerid " + XzyAuthApp.ownerid + ")");

        string u = "ctl" + Environment.TickCount.ToString("X");

        Console.Write("create(" + u + ") ");
        var c = XzyAuthApp.Create(u, "pass123", "30d");
        Console.WriteLine(c.ok ? "OK id=" + c.id : "FAIL: " + c.message);
        string id = c.id;

        Console.Write("login(" + u + ") ");
        XzyAuthApp.login(u, "pass123");
        Console.WriteLine(XzyAuthApp.response.success ? "LOGIN OK @ " + XzyAuthApp.response.expiry : "DENIED: " + XzyAuthApp.response.message);

        Console.Write("ban(" + u + ") ");
        XzyAuthApp.Ban(id, true);
        Console.WriteLine(XzyAuthApp.response.message);

        Console.Write("login(" + u + ") again ");
        XzyAuthApp.login(u, "pass123");
        Console.WriteLine(XzyAuthApp.response.success ? "OK (UNEXPECTED)" : "DENIED: " + XzyAuthApp.response.message);

        Console.Write("unban(" + u + ") ");
        XzyAuthApp.Ban(id, false);
        Console.WriteLine(XzyAuthApp.response.message);

        Console.Write("delete(" + u + ") ");
        var d = XzyAuthApp.Delete(id);
        Console.WriteLine(d.ok ? "DELETED" : "FAIL: " + d.message);

        Console.Write("login(" + u + ") after delete ");
        XzyAuthApp.login(u, "pass123");
        Console.WriteLine(XzyAuthApp.response.success ? "OK (UNEXPECTED)" : "DENIED: " + XzyAuthApp.response.message);

        Console.Write("CreateKey(7d) ");
        string lk = XzyAuthApp.CreateKey("7d");
        Console.WriteLine(lk.Length > 0 ? lk : "FAIL");
        if (lk.Length > 0)
        {
            Console.Write("license(" + lk + ") ");
            XzyAuthApp.license(lk);
            Console.WriteLine(XzyAuthApp.response.success ? "LOGIN OK @ " + XzyAuthApp.response.expiry : "DENIED: " + XzyAuthApp.response.message);
        }

        Console.WriteLine("== FULL CONTROL OK ==");
    }
}