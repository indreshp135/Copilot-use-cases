using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace VulnerableApp
{
    public class VulnerableClass
    {
        // VULNERABILITY #1: SQL Injection
        public string GetUserData(string userId)
        {
            string connectionString = "Server=localhost;Database=Users;Trusted_Connection=true;";
            string query = "SELECT * FROM Users WHERE UserId = '" + userId + "'";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                
                if (reader.Read())
                {
                    return reader["Name"].ToString();
                }
            }
            return "";
        }

        // VULNERABILITY #2: Hardcoded Credentials
        private string GetDatabasePassword()
        {
            return "SuperSecretPassword123!";
        }

        // VULNERABILITY #3: Path Traversal
        public string ReadFile(string fileName)
        {
            string basePath = @"C:\Users\Public\Documents\";
            string fullPath = basePath + fileName;
            
            if (File.Exists(fullPath))
            {
                return File.ReadAllText(fullPath);
            }
            return "File not found";
        }

        // VULNERABILITY #4: Weak Cryptography
        public string EncryptPassword(string password)
        {
            // Using weak MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // VULNERABILITY #5: XML External Entity (XXE) Injection
        public string ParseXml(string xmlContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = new XmlUrlResolver(); // Dangerous!
            doc.LoadXml(xmlContent);
            
            XmlNodeList nodes = doc.SelectNodes("//user");
            StringBuilder result = new StringBuilder();
            
            foreach (XmlNode node in nodes)
            {
                result.AppendLine(node.InnerText);
            }
            
            return result.ToString();
        }

        // VULNERABILITY #6: Command Injection
        public string ExecuteCommand(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c " + command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        // VULNERABILITY #7: Insecure Deserialization
        public object DeserializeData(string jsonData)
        {
            // Using BinaryFormatter which is dangerous
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = 
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            
            byte[] bytes = Convert.FromBase64String(jsonData);
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return formatter.Deserialize(stream);
            }
        }

        // VULNERABILITY #8: Information Disclosure
        public string GetSystemInfo()
        {
            string info = "";
            info += "OS Version: " + Environment.OSVersion + "\n";
            info += "Machine Name: " + Environment.MachineName + "\n";
            info += "User Name: " + Environment.UserName + "\n";
            info += "Current Directory: " + Environment.CurrentDirectory + "\n";
            info += "System Directory: " + Environment.SystemDirectory + "\n";
            return info;
        }

        // VULNERABILITY #9: Race Condition
        private static int counter = 0;
        public void IncrementCounter()
        {
            // Race condition - not thread safe
            counter++;
        }

        // VULNERABILITY #10: Buffer Overflow (simulated)
        public string CreateLargeString(int size)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                sb.Append("A");
            }
            return sb.ToString();
        }

        // VULNERABILITY #11: Insecure Random Number Generation
        public int GenerateRandomNumber()
        {
            Random random = new Random(); // Not cryptographically secure
            return random.Next(1, 100);
        }

        // VULNERABILITY #12: Missing Input Validation
        public string ProcessUserInput(string userInput)
        {
            // No validation of user input
            return "Processed: " + userInput;
        }

        // VULNERABILITY #13: Insecure File Upload
        public void SaveUploadedFile(byte[] fileData, string fileName)
        {
            string uploadPath = @"C:\uploads\" + fileName;
            File.WriteAllBytes(uploadPath, fileData);
        }

        // VULNERABILITY #14: Cross-Site Scripting (XSS) - reflected
        public string EchoUserInput(string userInput)
        {
            // Directly echoing user input without sanitization
            return "<html><body>You said: " + userInput + "</body></html>";
        }

        // VULNERABILITY #15: Insecure Direct Object Reference
        public string GetUserById(int userId)
        {
            // No authorization check
            string connectionString = "Server=localhost;Database=Users;Trusted_Connection=true;";
            string query = "SELECT * FROM Users WHERE Id = " + userId;
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                
                if (reader.Read())
                {
                    return reader["Name"].ToString();
                }
            }
            return "";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            VulnerableClass vuln = new VulnerableClass();
            
            Console.WriteLine("Vulnerable Application Demo");
            Console.WriteLine("==========================");
            
            // Demonstrate some vulnerabilities
            Console.WriteLine("1. SQL Injection vulnerability:");
            string result1 = vuln.GetUserData("1' OR '1'='1");
            Console.WriteLine("Result: " + result1);
            
            Console.WriteLine("\n2. Path Traversal vulnerability:");
            string result2 = vuln.ReadFile("..\\..\\..\\Windows\\System32\\drivers\\etc\\hosts");
            Console.WriteLine("Result: " + result2);
            
            Console.WriteLine("\n3. Weak cryptography:");
            string result3 = vuln.EncryptPassword("password123");
            Console.WriteLine("MD5 Hash: " + result3);
            
            Console.WriteLine("\n4. Information disclosure:");
            string result4 = vuln.GetSystemInfo();
            Console.WriteLine("System Info: " + result4);
            
            Console.WriteLine("\n5. Insecure random generation:");
            int result5 = vuln.GenerateRandomNumber();
            Console.WriteLine("Random number: " + result5);
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
