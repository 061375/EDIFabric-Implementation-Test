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
    static class Constants
    {

    }
    class Program
    {

        private static bool debug = false;

        private static string theClass = "";

        private static string theFile = "";

        private static string thePortID = "";

        private static string theStatus = "";

        public static readonly string currentDBconn = ConfigurationManager.AppSettings["dbConn"] + "";

        public static readonly string pathToFiles = ConfigurationManager.AppSettings["filePath"];

        static void Main(string[] args)
        {
            Console.WriteLine(currentDBconn);
            // get the flags from cmd
            GetArgs(args);
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
                            Libraries.Writers.Types.X12850 _X12850 = new Libraries.Writers.Types.X12850(theFile, thePortID);
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
                        case "-f":
                            theFile = args[i];
                            break;
                        case "-s":
                            theStatus = args[i];
                            break;
                    }
                }
            }
        }
        /**
         * Adds the gathered and normalized EDI data into the database
         * 
         * @param class: interface EDIextraction
         * */
        static void PutData(IEDIextraction _class)
        {
            Libraries.Data.Put p = new Libraries.Data.Put(_class, currentDBconn);
        }
    }

}