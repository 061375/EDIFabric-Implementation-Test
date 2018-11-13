using EDI.Fabric.Libraries.Helpers;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Framework;
using EdiFabric.Framework.Readers;
using EdiFabric.Sdk.Helpers;
using EdiFabric.Templates.X12;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace EDI.Fabric
{

    class Program
    {
        public static Dictionary<string, string> CustomerConfig = null;

        public static bool debug = false;

        public static string theClass = "";

        private static string theFile = "";

        public static string thePortID = "";

        private static string theSSCC = "";

        private static string theStatus = "";

        private static string thePO = "";

        public static readonly string currentDBconn = ConfigurationManager.AppSettings["dbConn"] + "";

        public static string pathToFiles = ConfigurationManager.AppSettings["filePath"];

        static string RawPathToFile = null;

        //@todo - make these overideable with a config loaded by argument
        public static readonly string ShipFromCompanyName = ConfigurationManager.AppSettings["ShipFromCompanyName"];

        public static readonly string ShipFromCompanyAddress = ConfigurationManager.AppSettings["ShipFromCompanyAddress"];

        public static readonly string ShipFromCompanyCity = ConfigurationManager.AppSettings["ShipFromCompanyCity"];

        public static readonly string ShipFromCompanyState = ConfigurationManager.AppSettings["ShipFromCompanyState"];

        public static readonly string ShipFromCompanyZip = ConfigurationManager.AppSettings["ShipFromCompanyZip"];

        public static readonly string ShipFromCompanyCountry = ConfigurationManager.AppSettings["ShipFromCompanyCountry"];

        public static string pathToFile = "";

        static void Main(string[] args)
        {

            // get the flags from cmd
            GetArgs(args);
            //
            // 
            GetConfigMain();
            //

            switch (theStatus)
            {
                case "Receive":
                    switch (theClass)
                    {
                        case "850":
                            Libraries.Readers.Types.X12850 _X12850 = new Libraries.Readers.Types.X12850(theFile, thePortID);
                            PutData(_X12850);
                            if (debug) _X12850.GetXML();
                            break;
                        case "856":
                            Libraries.Readers.Types.X12856 _X12856 = new Libraries.Readers.Types.X12856(theFile, thePortID);
                            //PutData(_X12850);
                            if (debug) _X12856.GetXML();
                            break;
                        case "810":
                            Libraries.Readers.Types.X12810 _X12810 = new Libraries.Readers.Types.X12810(theFile, thePortID);
                            PutData(_X12810);
                            if (debug) _X12810.GetXML();
                            break;
                        default:
                            Console.WriteLine("The class was not specified or does not exist");
                            break;
                    }
                    break;
                case "Send":
                    switch (theClass)
                    {
                        case "850":
                            Libraries.Writers.Types.X12850 _X12850 = new Libraries.Writers.Types.X12850(thePortID,thePO, theSSCC);
                            break;
                        case "856":
                            Libraries.Writers.Types.X12856 _X12856 = new Libraries.Writers.Types.X12856(thePortID, thePO, theSSCC);
                            break;
                        case "810":
                            break;
                        default:
                            Console.WriteLine("The class was not specified or does not exist");
                            break;
                    }
                    break;
            }
        }
        static void GetArgs(string[] args)
        {
            // @TODO - many of these flags should overide settings stored in a db called by the Port ID

            string flag = "";
            for(int i=0; i<args.Length; i++)
            {
                if(args[i].IndexOf("-") > -1)
                {
                    flag = args[i];
                    // this is because there may be no other argument to loop to
                    if("-d" == flag) debug = true;
                }
                else
                {
                    switch(flag)
                    {
                        case "-d":
                            debug = true;
                            break;
                        case "-c":  
                            theClass = args[i];
                            break;
                        case "-p":
                            thePortID = args[i];
                            break;
                        case "-sscc":
                            theSSCC = args[i];
                            break;
                        case "-f":
                            theFile = args[i];
                            break;
                        case "-pf":
                            pathToFiles = args[i];
                            break;
                        case "-rpf":
                            RawPathToFile = args[i];
                            break;
                        case "-s":
                            theStatus = args[i];
                            break;
                        case "-po":
                            break;
                        case "-h":
                            string helpmenu = "flag [arg type] \n";
                            helpmenu += "-d [null] : dumps debug data and dumps EDI data as XML \n";
                            helpmenu += "-c [string] the edi class to use \b";
                            helpmenu += "-p [string] the port id \n";
                            helpmenu += "-pf [string] overide default path to files \n";
                            helpmenu += "-rpf [string] raw path to file example: C:/path/to/file.edi \n";
                            helpmenu += "-sscc [string] the sscc \n";
                            helpmenu += "-f [string] name of the EDI file to target \n";
                            helpmenu += "-s [string] whether the operation is getting or setting data \n";
                            helpmenu += "            Send, Receive, Respond \n";
                            helpmenu += "-po [string] po number";
                            EchoResponseAndDie(helpmenu);
                            break;
                    }
                }
            }

            // build path to files
            pathToFile = pathToFiles + theStatus + "/" + theFile;

            // ----- HANDLE DEFAULTS

            // if user wants raw file path
            if (null != RawPathToFile)
            {
                pathToFile = RawPathToFile;
            }

            // ----- DEBUG

            if (debug)
            {
                Console.WriteLine("currentDBconn " + currentDBconn);
                Console.WriteLine("theStatus " + theStatus);
                Console.WriteLine("theFile " + theFile);
                Console.WriteLine("thePortID " + thePortID);
                Console.WriteLine("theSCCC " + theSSCC);
                Console.WriteLine("thePO " + thePO);
                Console.WriteLine("RawPathToFile " + RawPathToFile);
                Console.WriteLine("pathToFile " + pathToFile);
            }
            else
            {
                string[] log =
                {
                    "currentDBconn " + currentDBconn,
                    "theStatus " + theStatus,
                    "theFile " + theFile,
                    "thePortID " + thePortID,
                    "theSCCC " + theSSCC,
                    "thePO " + thePO,
                    "RawPathToFile " + RawPathToFile,
                    "pathToFile " + pathToFile
                };
                Helpers.Log(log);
            }
        }
        /**
         * Adds the gathered and normalized EDI data into the database
         * 
         * @param class: interface EDIextraction
         * */
        static void PutData(IEDIextraction _class)
        {
            Libraries.Data.Set p = new Libraries.Data.Set(_class, currentDBconn);
        }
        static void GetConfigMain()
        {
            using(Libraries.Data.Get get = new Libraries.Data.Get(currentDBconn))
            {
                CustomerConfig = get.GetConfigMain(thePortID);
            }
        }
        static void EchoResponseAndDie(string s)
        {
            Console.WriteLine(s);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }

}