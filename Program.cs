using System;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace CWRGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up the CWR file header
            StringBuilder cwrData = new StringBuilder();
            Random rnd = new Random();

            // HDR and SenderType Hardcoded

            // SenderID - Ask for a 9 digit sender ID or randomly generate

            string senderId;

            while (true)
            {
                Console.WriteLine("-- HDR Generation --");
                Console.WriteLine("SENDER ID");
                Console.Write("Enter 'R' to generate a random sender ID or 'I' to input a sender ID: ");
                string senderIdOption = Console.ReadLine();

                if (senderIdOption == "R")
                {
                    senderId = GenerateRandomIPINumber(rnd);
                    break;
                }
                else if (senderIdOption == "I")
                {
                    Console.Write("Enter the sender ID (9 digits): ");
                    senderId = Console.ReadLine();

                    if (senderId.Length > 9)
                    {
                        Console.WriteLine("The sender ID cannot be more than 9 digits. Please enter a valid sender ID.");
                    }
                    else
                    {
                        senderId = senderId.PadLeft(9, '0');
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Please enter 'R' to generate a random sender ID or 'I' to input a sender ID.");
                }
            }

            // Ask user for a sender name 
            string senderName = "";

            // Accept user input for the sender name
            while (true)
            {
                Console.WriteLine("SENDER NAME");
                Console.Write("Enter the sender name (up to 45 characters): ");
                senderName = Console.ReadLine();

                if (string.IsNullOrEmpty(senderName))
                {
                    //Error handling
                    Console.WriteLine("The sender name cannot be empty. Please enter a valid sender name.");
                }
                else
                {
                    // Pad the sender name with spaces if necessary
                    if (senderName.Length < 45)
                    {
                        senderName = senderName.PadRight(45);
                    }
                    break;
                }
            }

            // Create timing fields
            DateTime now = DateTime.Now;
            string creationDate = now.ToString("yyyyMMdd");
            string creationTime = now.ToString("HHmmss");
            string transmissionDate = creationDate;

            cwrData.AppendLine($"HDRPB{senderId}{senderName}01.10{creationDate}{creationTime}{transmissionDate}");

            // Generate the Group Header line
            cwrData.AppendLine("GRHNWR0000102.10");

            // Generate a random number of musical works
            int numWorks;

            while (true)
            {
                // Ask if random number or specified number of works is required
                Console.WriteLine("--WORKS GENERATION--");
                Console.Write("Enter 'R' to generate a random number of musical works or 'S' to specify the number of works: ");
                string worksOption = Console.ReadLine();

                if (worksOption == "R")
                {
                    // If random specify min max range
                    Console.Write("Enter the minimum number of musical works: ");
                    int minWorks = int.Parse(Console.ReadLine());

                    Console.Write("Enter the maximum number of musical works: ");
                    int maxWorks = int.Parse(Console.ReadLine());

                    numWorks = rnd.Next(minWorks, maxWorks + 1);
                    break;
                }
                else if (worksOption == "S")
                {
                    // If specified number of works chosen, input range
                    Console.Write("Enter the number of musical works: ");
                    string worksInput = Console.ReadLine();
                    if (!int.TryParse(worksInput, out numWorks))
                    {
                        //Error handling
                        Console.WriteLine("Invalid input. The number of musical works must be an integer.");
                    }
                    else
                    {
                        break;
                    }
                    // Error handling
                    Console.WriteLine("Invalid option. Please enter 'R' to generate a random number of musical works or 'S' to specify the number of works.");
                }
            }

            int transactionSeqNum = 0;
            int recordSeqNum = 0; 

            for (int i = 0; i < numWorks; i++)
            {
                // Generate a random title
                string title = GenerateRandomString(rnd, 40);
                title = title.PadRight(40);

                // Generate a random submitter work number
                string submitterWorkNumber = GenerateRandomString(rnd, 14);
                submitterWorkNumber = submitterWorkNumber.PadRight(14);

                // Generate a random writer
                string writer = GenerateRandomString(rnd, 20);

                // Generate a random publisher
                string publisher = GenerateRandomString(rnd, 20);

                // Generate a random IPI name number
                string ipiNameNumber = GenerateRandomIPINumber(rnd);

                // Add the musical work data to the CWR file
                cwrData.AppendLine($"NWR{transactionSeqNum:D8}{recordSeqNum:D8}{title}  {submitterWorkNumber}              POP      Y   ORI");                
                recordSeqNum++;
                cwrData.AppendLine($"PU{transactionSeqNum:D8}{recordSeqNum:D8}{writer}");               
                recordSeqNum++;
                cwrData.AppendLine($"IP{transactionSeqNum:D8}{recordSeqNum:D8}{publisher}");                             
                recordSeqNum = 0;
                transactionSeqNum++;
            }


            // Save the CWR file to disk

            string fileName;
            while (true)
            {
                // Ask for filename
                Console.WriteLine("Please name the file");
                fileName = Console.ReadLine();

                // Error handling
                if (String.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Name cannot be empty.  Please name the file.");
                }
                else
                {
                    File.WriteAllText($"{fileName}.V21", cwrData.ToString());
                    break;
                }
            }                                                  
        }

        static string GenerateRandomString(Random rnd, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        static string GenerateRandomIPINumber(Random rnd)
        {
            return $"{rnd.Next(100000000, 999999999):D8}";
        }        
    }
};

