using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Managers
{
    public class FileUploadManager
    {
        // fields
        private List<string> _validExtensions;
        private readonly IHostingEnvironment _hostingEnvironment;

        // ctors
        public FileUploadManager(IHostingEnvironment hostingEnvironment, params string[] extensions)
        {
            _hostingEnvironment = hostingEnvironment;
            _validExtensions = extensions.ToList();
        }

        // methods (public to private)
        public string Upload(string path, string base64File, string extension, string tag, string directory = null, string previousFile = null)
        {
            if (!ValidateExtensionForFiles(extension))
                return null;

            //CreateDirectoryIfNotExists(path);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                path += $"/{MakeDirectoryName(directory)}";
                if (!Directory.Exists($"{_hostingEnvironment.WebRootPath}/{path}"))
                    Directory.CreateDirectory($"{_hostingEnvironment.WebRootPath}/{path}");
                path += "/";
            }

            string fileTime = DateTime.Now.ToFileTime().ToString();
            string withFileTimeName = $"{fileTime}-{MakeDirectoryName(tag)}.{extension}";

            string filePath = $"{_hostingEnvironment.WebRootPath}{path}{withFileTimeName}";

            string dbPath = $"{path}{withFileTimeName}";

            filePath = ReplaceTrChars(filePath);
            dbPath = ReplaceTrChars(dbPath);

            var previousFilePath = $"{path}/{previousFile}";
            if (previousFile != null && File.Exists(previousFilePath))
                File.Delete(previousFilePath);

            byte[] fileContent = ConvertBase64StringToByteArray(base64File);

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileContent, 0, fileContent.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return dbPath;
        }
        public void Delete(string dbPath)
        {
            var path = $"{_hostingEnvironment.WebRootPath}{dbPath}";
            if (File.Exists(path))
                File.Delete(path);
        }
        private byte[] ConvertBase64StringToByteArray(string base64File)
        {
            int index = base64File.IndexOf(',');

            if (index != -1)
                base64File = base64File.Replace(base64File.Substring(0, index + 1), "");

            return Convert.FromBase64String(base64File);
        }
        private bool CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return true;
            }
            return false;
        }
        private void InitializeValidExtensions()
        {
            _validExtensions = new List<string>() {
                "jpeg", "jpg", "png"
            };
        }
        private bool ValidateExtensionForFiles(string extension)
        {
            extension = extension.ToLower();
            foreach (var ext in _validExtensions)
                if (extension == ext)
                    return true;
            return false;
        }
        private string ClearIllegalChars(string text)
        {
            return text.Replace("\"", "")
                .Replace("\'", "")
                .Replace("\"", "")
                .Replace("#", "")
                .Replace("!", "")
                .Replace("$", "")
                .Replace("%", "")
                .Replace("&", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("/", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("=", "")
                .Replace("?", "")
                .Replace("*", "");
        }
        public string MakeDirectoryName(string name)
        {
            name = name.ToLower();
            name = ReplaceTrChars(name);
            name = ClearIllegalChars(name);
            name = name.Replace("-", "");
            var splitted = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var builder = new StringBuilder();
            for (int i = 0; i < splitted.Length; i++)
            {
                if (i != splitted.Length - 1)
                    builder.Append($"{splitted[i]}-");
                else
                    builder.Append(splitted[i]);
            }

            return builder.ToString();
        }
        public string ReplaceTrChars(string text)
        {
            return text.Replace('ı', 'i')
                .Replace('ğ', 'g')
                .Replace('ü', 'u')
                .Replace('ş', 's')
                .Replace('ö', 'o')
                .Replace('ç', 'c')
                .Replace('İ', 'I')
                .Replace('Ü', 'U')
                .Replace('Ş', 'S')
                .Replace('Ö', 'O')
                .Replace('Ç', 'C');
        }
    }
}

//public string Upload(string path, string base64File, string extension, string previousFile = null, string preDeterminedName = null)
//{
//    if (!ValidateExtensionForFiles(extension))
//        return null;

//    CreateDirectoryIfNotExists(path);

//    string fileTime = DateTime.Now.ToFileTime().ToString();
//    string filePath = "";
//    string dbPath = "";
//    string withFileTimeName = $"{fileTime.ToString()}.{extension}";
//    string withPreDeterminedName = $"{preDeterminedName}_{fileTime}.{extension}";
//    withPreDeterminedName = ClearIllegalChars(withPreDeterminedName);
//    preDeterminedName = preDeterminedName != null ? ClearIllegalChars(preDeterminedName) : null;

//    filePath = string.IsNullOrWhiteSpace(preDeterminedName) ?
//        $"{path}{fileTime}.{extension}"
//        :
//        $"{path}{preDeterminedName}_{fileTime}.{extension}";

//    dbPath = string.IsNullOrWhiteSpace(preDeterminedName) ?
//        $"{path}/{withFileTimeName}"
//        :
//        $"{path}/{withPreDeterminedName}";

//    filePath = ReplaceTrChars(filePath);
//    dbPath = ReplaceTrChars(dbPath);

//    var previousFilePath = $"{path}/{previousFile}";
//    if (previousFile != null && File.Exists(previousFilePath))
//        File.Delete(previousFilePath);

//    byte[] fileContent = ConvertBase64StringToByteArray(base64File);

//    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
//    {
//        fs.Write(fileContent, 0, fileContent.Length);
//    }

//    return dbPath;
//}