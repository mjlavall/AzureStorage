using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AzureStorageStreaming.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureStorageStreaming.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var files = new Dictionary<string, string>();
            foreach (var blob in CloudAccess.GetFiles())
            {
                if (blob is CloudBlockBlob)
                {
                    var blockBlob = (CloudBlockBlob)blob;
                    files.Add(blockBlob.Name, blockBlob.Uri.ToString());
                }
            }
            return View(files);
        }
        // GET: /Home/Player/GOPRO015.MP4
        public ActionResult Player(string id)
        {
            ViewBag.FileName = id;
            return View();
        }
    }
}
