using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assignment1
{
    class Program
    {
       
        static string filePath = "Assignment1.json";

        /// <summary>
        /// Main routine
        /// </summary>
        /// <param name="args">filePath</param>
        static void Main(string[] args)
        {          
            
            if( (args != null) && (args.Length > 0))
                filePath = args[0];
            else
                Console.WriteLine("Usage: Assignment1.exe [PATH TO FILE Assignment1.json]");

            Dictionary<int, Dictionary<string, string>> parsedFile = null;
            StringBuilder sb = new StringBuilder();  

            try
            {
                parsedFile = JsonImporter.ImportFile(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Upps! There was a problem during parsing of the file: {0}", ex.Message);
            }

            try
            {
                parsedFile.Remove(parsedFile.First().Key); //remove first entry (header values from csv file)
                TurtleGenerator gen = new TurtleGenerator(new DisasterMapper(), sb);
                gen.TransformToTurtleRdf(parsedFile);
                System.IO.File.WriteAllText(String.Format("{0}.ttl", filePath.Replace(System.IO.Path.GetExtension(filePath),"")), sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Upps! There was a problem during the generation of the turtle file: {0}", ex.Message);
            }            

            Console.WriteLine("FINISHED! Press [Key] to exit...");
            Console.ReadKey();

        }

       

       

       

      
    }
}
