﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CMS.Models;
using HVIT.Security;

namespace CMS.Controllers
{
    [RoutePrefix("fileupload")]
    public class FileUploadController : ApiController
    {
        public static string RamdomString(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i*temp*((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(36);
                if (temp != -1 && temp == t)
                {
                    return RamdomString(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        [Route("upload")]
        [AuthorizeUser, HttpPost]
        public async Task<HttpResponseMessage> UploadFiles()
        {
            var db = new ApplicationDbContext();

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/FileUpload");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                string key = "", type="";
                foreach (MultipartFileData file in provider.FileData)
                {
                    var fileName = file.Headers.ContentDisposition.FileName.Replace("\"", "").Split('.');
                    var sourcePath = file.LocalFileName;
                    var newName = RamdomString(20);
                    var directory = Path.GetDirectoryName(sourcePath);
                    var destinationPath = Path.Combine(directory, newName + '.' + fileName[1]);
                    while (System.IO.File.Exists(destinationPath))
                    {
                        newName = RamdomString(20);
                        destinationPath = Path.Combine(directory, newName + '.' + fileName[1]);
                    }
                    File.Move(sourcePath, destinationPath);
                    var fileUpload = new FileUpload();
                    fileUpload.FileName = fileName[0];
                    fileUpload.FilePath = destinationPath;
                    fileUpload.FileSize = new FileInfo(destinationPath).Length.ToString();
                    fileUpload.FileKey = newName;
                    key = fileUpload.FileKey;
                    fileUpload.FileType = fileName[1];
                    type = fileUpload.FileType;
                    db.FileUpload.Add(fileUpload);
                }

                db.SaveChanges();
                string[] strType = { "mp3", "wma", "wav", "flac", "aac", "ogg", "aiff", "alac", "amr", "midi" };

                if (strType.Contains(type))
                    return Request.CreateResponse(HttpStatusCode.OK, key + "." + type);
                return Request.CreateResponse(HttpStatusCode.OK, key);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        
    }
}
