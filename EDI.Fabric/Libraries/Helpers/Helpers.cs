using EdiFabric.Core.Model.Edi.X12;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Helpers
{
    class Helpers
    {
        public static void Log(string[] lines) {
            // init paths if they don't exists
            string portid = Program.thePortID;
            if(null == portid)
            {
                portid = "null";
            }
            string theclass = Program.theClass;
            if (null == theclass)
            {
                theclass = "null";
            }
            // create a log directory
            string logdir = System.IO.Directory.GetCurrentDirectory() + "/logs";
            // "
            System.IO.Directory.CreateDirectory(logdir);
            // create the path to the log file
            string logfile = logdir + "/" + portid + "_" + theclass + "_log.txt";

            // show the path to the log in the console 
            Console.WriteLine("Attempting to write to: "+logfile);

            // new file create else append
            if (!File.Exists(logfile))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(logfile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss A"));

                    foreach (string line in lines)
                    {
                        sw.WriteLine(line);
                    }

                    sw.WriteLine("--------------------------");
                }
            }
            else
            {
                // Create a file to write to.
                using (StreamWriter sw = File.AppendText(logfile))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss A"));

                    foreach (string line in lines)
                    {
                        sw.WriteLine(line);
                    }

                    sw.WriteLine("--------------------------");
                }
            }
        }

        /// 
        /// Build ISA.
        /// 
        public static ISA BuildIsa(string controlNumber,
           string senderId = null,
           string senderQ = null,
           string receiverId = null,
           string receiverQ = null,
           string icvn = null,
           string ackRequested = "1",
           string testIndicator = "T")
        {
            if(null == senderId)
            {
                senderId = Program.CustomerConfig["InterchangeSID"];
            }
            if(null == senderQ)
            {
                senderQ = Program.CustomerConfig["InterchangeSIDQ"];
            }
            if (null == receiverQ)
            {
                receiverId = Program.CustomerConfig["InterchangeRIDQ"];
            }
            if (null == receiverId)
            {
                receiverId = Program.CustomerConfig["InterchangeRID"];
            }
            if (null == icvn)
            {
                receiverId = "00401";
            }
            return new ISA
            {
                //  Authorization Information Qualifier
                AuthorizationInformationQualifier_1 = Program.CustomerConfig["AuthIQ"],
                //  Authorization Information
                AuthorizationInformation_2 = Program.CustomerConfig["AuthI"].PadRight(10),
                //  Security Information Qualifier
                SecurityInformationQualifier_3 = Program.CustomerConfig["SecIQ"],
                //  Security Information
                SecurityInformation_4 = Program.CustomerConfig["SECI"].PadRight(10),
                //  Interchange ID Qualifier
                SenderIDQualifier_5 = senderQ,
                //  Interchange Sender
                InterchangeSenderID_6 = senderId.PadRight(15),
                //  Interchange ID Qualifier
                ReceiverIDQualifier_7 = receiverQ,
                //  Interchange Receiver
                InterchangeReceiverID_8 = receiverId.PadRight(15),
                //  Date
                InterchangeDate_9 = DateTime.Now.Date.ToString("yyMMdd"),
                //  Time
                InterchangeTime_10 = DateTime.Now.TimeOfDay.ToString("hhmm"),
                //  Standard identifier
                InterchangeControlStandardsIdentifier_11 = Program.CustomerConfig["RepSep"],
                //  Interchange Version ID
                //  This is the ISA version and not the transaction sets versions
                InterchangeControlVersionNumber_12 = icvn,
                //  Interchange Control Number
                InterchangeControlNumber_13 = controlNumber.PadLeft(9, '0'),
                //  Acknowledgment Requested (0 or 1)
                AcknowledgementRequested_14 = ackRequested,
                //  Test Indicator
                UsageIndicator_15 = testIndicator,
            };
        }
        /// 
        /// Build GS.
        /// 
        public static GS BuildGs(string controlNumber,
           string ciit = null,
           string senderId = null,
           string receiverId = null)
        {
            if (null == ciit)
            {
                ciit = "ST";
            }
            if (null == senderId)
            {
                senderId = Program.CustomerConfig["InterchangeSID"];
            }
            if (null == receiverId)
            {
                receiverId = Program.CustomerConfig["InterchangeRID"];
            }
            return new GS
            {
                //  Functional ID Code
                CodeIdentifyingInformationType_1 = ciit,
                //  Application Senders Code
                SenderIDCode_2 = senderId,
                //  Application Receivers Code
                ReceiverIDCode_3 = receiverId,
                //  Date
                Date_4 = DateTime.Now.Date.ToString("yyMMdd"),
                //  Time
                Time_5 = DateTime.Now.TimeOfDay.ToString("hhmm"),
                //  Group Control Number
                //  Must be unique to both partners for this interchange
                GroupControlNumber_6 = controlNumber.PadLeft(9, '0'),
                //  Responsible Agency Code
                TransactionTypeCode_7 = Constants.X12ISAGS7,
                //  Version/Release/Industry id code
                VersionAndRelease_8 = Constants.X12ISAGS8
            };
        }
    }
}
