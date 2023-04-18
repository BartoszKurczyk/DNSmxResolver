﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSmxV2
{
    public static class ProgramArgumensResolver
    {
        public static List<string> hosts = new List<string>();
        public static string inputFile = string.Empty;
        public static string outputFile = string.Empty;
        public static string dns = string.Empty;
        public static bool argsValid = false;
        public static string errorMessage = string.Empty;

        public static void ResolveArgs(string[] args)
        {
            ValidateArgs(args);
            if (!argsValid) return;
            var argsList = args.ToList();
            for (int i = 0; i < argsList.Count; i++)
            {
                switch (argsList[i])
                {
                    case "-dns":
                        dns = argsList[i + 1];
                        i++;
                        break;
                    case "-input":
                        inputFile = argsList[i + 1];
                        i++;
                        break;
                    case "-output":
                        outputFile = argsList[i + 1];
                        i++;
                        break;
                    default:
                        hosts.Add(argsList[i]);
                        break;
                }
            }

            if(inputFile!= string.Empty)
            {
                hosts.Clear();
                string textFromFile = File.ReadAllText(inputFile);

                hosts.AddRange(textFromFile.Split(";"));
            }
        }
        private static void ValidateArgs(string[] args)
        {
            var argsList = args.ToList();

            if (argsList.FindAll(x => x.Equals("-dns")).Count > 1)
            {
                argsValid = false;
                errorMessage = "There is more then one -dns switches";
                return;
            }
            if (argsList.FindAll(x => x.Equals("-input")).Count > 1)
            {
                argsValid = false;
                errorMessage = "There is more then one -input switches";
                return;
            }
            if (argsList.FindAll(x => x.Equals("-output")).Count > 1)
            {
                argsValid = false;
                errorMessage = "There is more then one -output switches";
                return;
            }

            if(argsList.Count==0)
            {
                argsValid = false;
                errorMessage = "There is zero arguments";
                return;
            }

            argsValid = true;
        }
    }
}