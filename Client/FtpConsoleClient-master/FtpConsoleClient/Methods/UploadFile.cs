using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ftpConsoleClient.Methods
{
	public class UploadFile : AbstractFtpMethod
	{
		private static byte[] FileToByteArray(string fileName)
		{
			FileStream fs = new FileStream(fileName,
				FileMode.Open,
				FileAccess.Read);
			var br = new BinaryReader(fs);
			var contentLength = new FileInfo(fileName).Length;
			return br.ReadBytes((int)contentLength);
		}

		public override void SendRequest(params string[] consoleArgs)
		{
			if (consoleArgs.Length == 0)
			{
				Console.WriteLine("Path to uploading file is requare!");
				return;
			}

			if (!File.Exists(consoleArgs[0]))
			{
				Console.WriteLine("Uploading file not exists...");
				return;
			}

			var fileUri = new Uri(ftpUri, Path.GetFileName(consoleArgs[0]));

			request = CreateFtpRequest(WebRequestMethods.Ftp.UploadFile, fileUri);
			Console.Write("Connecting to {0}...\n\n", ftpUri);

			// Copy the contents of the file to the request stream.
			var fileContents = FileToByteArray(consoleArgs[0]);

			request.ContentLength = fileContents.Length;

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(fileContents, 0, fileContents.Length);
			}

			using (var response = (FtpWebResponse) request.GetResponse())
			{
				Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
			}
		}
	}
}
