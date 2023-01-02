using System;
using System.IO;
using System.Linq;
using System.Text;

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
                    Console.Write("Enter the number of musical works: ");
                    string worksInput = Console.ReadLine();
                    if (!int.TryParse(worksInput, out numWorks))
                    {
                        Console.WriteLine("Invalid input. The number of musical works must be an integer.");
                    }
                    else
                    {
                        break;
                    }
                    Console.WriteLine("Invalid option. Please enter 'R' to generate a random number of musical works or 'S' to specify the number of works.");
                }
            }

            for (int i = 0; i < numWorks; i++)
            {
                // Generate a random title
                string title = GenerateRandomString(rnd, 20);

                // Generate a random composer
                string composer = GenerateRandomString(rnd, 20);

                // Generate a random publisher
                string publisher = GenerateRandomString(rnd, 20);

                // Generate a random rights holder
                string rightsHolder = GenerateRandomString(rnd, 20);

                // Generate a random IPI name number
                string ipiNameNumber = GenerateRandomIPINumber(rnd);

                // Generate a random copyright notice
                string copyrightNotice = GenerateRandomCopyrightNotice(rnd, title);

                // Generate a random licensing terms
                string licensingTerms = GenerateRandomLicensingTerms(rnd);

                // Add the musical work data to the CWR file
                cwrData.AppendLine($"WR{i + 1:D6}{title}");
                cwrData.AppendLine($"PU{composer}");
                cwrData.AppendLine($"IP{publisher}");
                cwrData.AppendLine($"RH{rightsHolder}");
                cwrData.AppendLine($"PN{ipiNameNumber}");
                cwrData.AppendLine($"CO{copyrightNotice}");
                cwrData.AppendLine($"LT{licensingTerms}");
            }


            // Save the CWR file to disk
            File.WriteAllText("cwr_file.txt", cwrData.ToString());
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

        static string GenerateRandomCopyrightNotice(Random rnd, string title)
        {
            int year = rnd.Next(1900, 2023);
            return $"Copyright {year} by {title}";
        }

        static string GenerateRandomLicensingTerms(Random rnd)
        {
            int numTerms = rnd.Next(1, 5);
            string[] terms = new string[numTerms];
            for (int i = 0; i < numTerms; i++)
            {
                terms[i] = GenerateRandomString(rnd, 15);
            }
            return string.Join("/", terms);
        }
    }
};

