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
                Console.WriteLine("--HDR Generation--");
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
            //Generate Work Information
            for (int i = 0; i < numWorks; i++)
            {
                //Initialize transaction sequence number
                int transactionSeqNum = i;

                //Initialize record sequence number
                int recordSeqNum = 0;

                //Initialize publisher sequence number
                int publisherSeqNum = 1;

                //Initialize territory sequence number
                int territorySeqNum = 1;

                //Setup Unknown Publisher
                string unknownPublisher = "Unknown Publisher";
                unknownPublisher = unknownPublisher.PadRight(45);

                // Generate a random title
                string title = GenerateRandomString(rnd, 40);
                title = title.PadRight(60);

                // Generate a random submitter work number
                string submitterWorkNumber = GenerateRandomString(rnd, 14);
                submitterWorkNumber = submitterWorkNumber.PadRight(14);

                // Generate a random writer lastname
                string writerLastName = GenerateRandomString(rnd, 20);
                writerLastName = writerLastName.PadRight(45);

                //Generate a random writer firstname
                string writerFirstName = GenerateRandomString(rnd, 20);
                writerFirstName = writerFirstName.PadRight(30);

                // Generate a random publisher
                string publisher = GenerateRandomString(rnd, 20);
                publisher = publisher.PadRight(45);

                // Generate a random IPI name number
                string ipiNameNumber = GenerateRandomIPINumber(rnd);
                ipiNameNumber = ipiNameNumber.PadLeft(11, '0');

                //Generate a random PR Ownership
                string prOwnership = GenerateRandomPRShare(rnd);
                int prOwnershipInt = int.Parse(prOwnership);

                //Generate a random MR Ownership
                string mrOwnership = GenerateRandomMRShare(rnd);
                int mrOwnsershipInt = int.Parse(mrOwnership);

                //Generate a random Interested Party Number For Publisher
                string interestedPartyNumPub = GenerateRandomString(rnd, 9);
                interestedPartyNumPub = interestedPartyNumPub.PadRight(9);

                //Generate a random Interested PArty Number For Writer
                string interestedPartyNumWriter = GenerateRandomString(rnd, 9);
                interestedPartyNumWriter = interestedPartyNumWriter.PadRight(9);

                // Add the musical work data to the CWR file
                //NWR Generation
                cwrData.AppendLine($"NWR{transactionSeqNum:D8}{recordSeqNum:D8}{title}  {submitterWorkNumber}                               POP      Y      ORI");                
                recordSeqNum++;

                //SPU Generation
                cwrData.AppendLine($"SPU{transactionSeqNum:D8}{recordSeqNum:D8}{publisherSeqNum:D2}{interestedPartyNumPub}{publisher} E          {ipiNameNumber}                 {prOwnership}   {mrOwnership}   00000");               
                recordSeqNum++;

                //SPT Generation
                cwrData.AppendLine($"SPT{transactionSeqNum:D8}{recordSeqNum:D8}{interestedPartyNumPub}      {prOwnership}{mrOwnership}00000I0124 {territorySeqNum:D3}");
                recordSeqNum++;
                
                //Unknown Publisher Generation
                //Check if MR Ownership is less than 10000

                if (mrOwnsershipInt < 10000)
                {
                    //Calculate difference
                    int diff = 10000 - mrOwnsershipInt;

                    //Add OPU line with difference
                    publisherSeqNum++;
                    string interestedPartyNumUnknownPub = GenerateRandomString(rnd, 9);
                    interestedPartyNumUnknownPub = interestedPartyNumUnknownPub.PadRight(9);
                    cwrData.AppendLine($"OPU{transactionSeqNum:D8}{recordSeqNum:D8}{publisherSeqNum:D2}{interestedPartyNumUnknownPub}{unknownPublisher} E                                      00000   {diff:D5}   00000");
                    recordSeqNum++;
                }

                //SWR Generation
                //calc leftover pr
                int diffpr = 10000 - prOwnershipInt;
                interestedPartyNumWriter = GenerateRandomString(rnd, 9);
                interestedPartyNumWriter = interestedPartyNumWriter.PadRight(9);
                ipiNameNumber = GenerateRandomIPINumber(rnd);
                ipiNameNumber = ipiNameNumber.PadLeft(11, '0');
                cwrData.AppendLine($"SWR{transactionSeqNum:D8}{recordSeqNum:D8}{interestedPartyNumWriter}{writerLastName}{writerFirstName} C          {ipiNameNumber}   {diffpr:D5}   00000   00000");
                recordSeqNum++;

                //SWT Generation
                territorySeqNum = 1;
                cwrData.AppendLine($"SWT{transactionSeqNum:D8}{recordSeqNum:D8}{interestedPartyNumWriter}{diffpr:D5}0000000000I0124 {territorySeqNum:D3}");
                recordSeqNum++;

                //PWR Generation
                cwrData.AppendLine($"PWR{transactionSeqNum:D8}{recordSeqNum:D8}{interestedPartyNumPub}{publisher}                            {interestedPartyNumWriter}");
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
            return $"{rnd.Next(100000000, 999999999):D9}";
        }        
        static string GenerateRandomPRShare(Random rnd)
        {
            return $"{rnd.Next(00000, 05000):D5}";
        }
        static string GenerateRandomMRShare(Random rnd)
        {
            return $"{rnd.Next(00000, 10000):D5}";
        }

    }
};

