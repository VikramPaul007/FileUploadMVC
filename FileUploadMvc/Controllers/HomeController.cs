using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FileUploadMvc.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FileUploadMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFiles(IEnumerable<IFormFile> files)
        {
            string Message = string.Empty;
            try
            {
                string FileUploadPath = Convert.ToString(_configuration.GetSection("FileUploadMvc").GetSection("FileUploadPath").Value.Trim());
                Int64 FileUploadMaxSize = Convert.ToInt64(_configuration.GetSection("FileUploadMvc").GetSection("FileUploadMaxSize").Value);
                string FileUploadAllowedTypes = Convert.ToString(_configuration.GetSection("FileUploadMvc").GetSection("FileUploadAllowedTypes").Value.ToLower().Trim());
                string FileUploadName = Convert.ToString(_configuration.GetSection("FileUploadMvc").GetSection("FileUploadName").Value.Trim());
                Dictionary<string, string> mimeTypeList = getAllowedMimeType(FileUploadAllowedTypes);

                foreach (IFormFile file in files)
                {
                    var contentType = file.ContentType;
                    var fileExtension = Path.GetExtension(file.FileName);

                    if (file.Length <= FileUploadMaxSize)
                    {
                        try
                        {
                            if (mimeTypeList.ContainsKey(contentType))
                            {
                                if (fileExtension == mimeTypeList[contentType])
                                {
                                    string filename = string.Concat(Guid.NewGuid(), Path.GetExtension(file.FileName));
                                    using (FileStream output = System.IO.File.Create(filename))
                                        await file.CopyToAsync(output);

                                    Message = String.Concat(Message, " Files Uploaded Successfully : ", file.FileName);
                                }
                                else
                                {
                                    Message = String.Concat(Message, " Upload Unsuccessful! File Content and File extension mismatch : ", file.FileName);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Message = String.Concat(Message, " ", e.Message, " FileName: ", file.FileName);
                        }
                    }
                    else
                    {
                        Message = String.Concat(Message, " File size exceeded max : ", file.FileName);
                    }
                }
            }
            catch (Exception e)
            {
                Message = String.Concat(Message, " ", e.Message);
            }
            ViewBag.Message = Message;
            return View("Index");
        }

        public Dictionary<string, string> getAllowedMimeType(string FileUploadAllowedTypes)
        {
            Dictionary<string, string> mimeTypeList = new Dictionary<string, string>();
            if (FileUploadAllowedTypes.Contains("jpeg"))
            {
                mimeTypeList.Add("image/jpeg", "jpeg");
            }
            if (FileUploadAllowedTypes.Contains("docx"))
            {
                mimeTypeList.Add("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx");
            }
            if (FileUploadAllowedTypes.Contains("doc"))
            {
                mimeTypeList.Add("application/msword", "doc");
            }
            if (FileUploadAllowedTypes.Contains("png"))
            {
                mimeTypeList.Add("image/png", "png");
            }
            if (FileUploadAllowedTypes.Contains("ppt"))
            {
                mimeTypeList.Add("application/vnd.ms-powerpoint", "ppt");
            }
            if (FileUploadAllowedTypes.Contains("pptx"))
            {
                mimeTypeList.Add("application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx");
            }
            if (FileUploadAllowedTypes.Contains("txt"))
            {
                mimeTypeList.Add("text/plain", "txt");
            }
            if (FileUploadAllowedTypes.Contains("xlsx"))
            {
                mimeTypeList.Add("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");
            }
            if (FileUploadAllowedTypes.Contains("zip"))
            {
                mimeTypeList.Add("application/zip", "zip");
            }
            return mimeTypeList;
        }
    }
}
