using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace FileIndexer.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = ParseParameters(args);
            var indexHolder = new IndexHolder();
            indexHolder.LoadIndexForFile(parameters.Filepath);

            var baseAddress = new Uri(string.Format("http://localhost:{0}/", parameters.Port));
            var indexService = new IndexService(new FileSource(parameters.Filepath, FixedParameters.Encoding), indexHolder);
            using (var host = new ServiceHost(indexService, baseAddress))
            {
                EnableMetadataPublishing(host);

                host.Open();

                Console.WriteLine("Service have following endpoints:");
                foreach (var endpoint in host.Description.Endpoints)
                {
                    Console.WriteLine("Binding: {1}, Address: {0}, Contract: {2}", endpoint.Address, endpoint.Binding.Name, endpoint.Contract.Name);
                }
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();
                
                host.Close();
            }
        }

        private static void EnableMetadataPublishing(ServiceHost host)
        {
            var smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    MetadataExporter = {PolicyVersion = PolicyVersion.Policy15}
                };
            host.Description.Behaviors.Add(smb);
        }

        private static Parameters ParseParameters(string[] args)
        {
            if (args.Length == 4)
            {
                var expectedPortParameter = false;
                var expectedFileParameter = false;
                var parameters = new Parameters();
                foreach (var arg in args)
                {
                    if (arg == "-port")
                    {
                        expectedPortParameter = true;
                    }
                    else if (arg == "-file")
                    {
                        expectedFileParameter = true;
                    }
                    else if (expectedFileParameter)
                    {
                        var filePath = arg;
                        if (!File.Exists(filePath))
                            throw new FileNotFoundException();

                        parameters.Filepath = filePath;
                        expectedFileParameter = false;
                    }
                    else if (expectedPortParameter)
                    {
                        parameters.Port = GetPort(arg);
                        expectedPortParameter = false;
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Invalid parameter {0}", arg), "args"); 
                    }
                }

                return parameters;
            }
            throw new ArgumentException("Expected 4 parameters: -port <port> -file <text file path>", "args");
        }

        private static string GetPort(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
                throw new ApplicationException("Port number is required");

            int result;
            if (!int.TryParse(arg, out result))
                throw new ArgumentException("Port number should be integer value");

            return arg;
        }
    }
}