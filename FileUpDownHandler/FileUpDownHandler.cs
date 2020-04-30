using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpDownHandler
{
    public class FileUpDownHandler
    {
        public ActionResponse Upload(string Domain,string UserId,string Password,)
        {
            //string login = ConfigurationManager.AppSettings["UploadAccessUserId"].ToString();
            //string domain = ConfigurationManager.AppSettings["UploadAccessDomainName"].ToString();
            //string password = Crypt.DecryptData(ConfigurationManager.AppSettings["UploadAccessPassword"].ToString());

    //<add key="UploadAccessImpersinationRequired" value="Y" />
    //<add key="UploadAccessUserId" value="SVC_CSMaqasaDocnew" />
    //<add key="UploadAccessDomainName" value="KGACHQ" />
    //<add key="UploadAccessPassword" value="MKjuOI9MZWxxD/StmFdCWqOCX+bngatC" />

            //string tokenId = "";
            //string mUserid = "";
            //string OrgReqId = "", OrgId = "";
            //string DocumentName = "";
            //string DocumentType = "";
            //string eservicerequestid = "";
            //bool ImportLicenseDoc = false;////added newly -  to differentiate if the upload is for Importer license details
            //string LicenseNumber = "", IssuanceDate = "", ExpiryDate = "", LicenseType = "";//added newly - form data passed in ajax request 
            //tokenId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tokenId"]);
            //mUserid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["mUserid"]);
            //DocumentName = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentName"]);
            //DocumentType = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentType"]);
            //ImportLicenseDoc = Convert.ToBoolean(System.Web.HttpContext.Current.Request.Form["ImportLicenseDoc"]);
            //string UploadedFrom = Convert.ToString(System.Web.HttpContext.Current.Request.Form["UploadedFrom"]);


            //eservicerequestid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]);
            //OrgReqId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["OrgReqId"]);
            //OrgId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["OrgId"]);

            if (eservicerequestid != null)
            {
                if (!eservicerequestid.All(char.IsDigit))
                {
                    eservicerequestid = CommonFunctions.CsUploadDecrypt(eservicerequestid.ToString());
                }
            }

            if (!OrgReqId.All(char.IsDigit))
            {

                OrgReqId = CommonFunctions.CsUploadDecrypt(OrgReqId.ToString());
            }



            if (!DocumentType.All(char.IsDigit))
            {
                DocumentType = CommonFunctions.CsUploadDecrypt(DocumentType);
            }
            MobileDataBase.Result rslt = new MobileDataBase.Result();
            if (UploadedFrom != "BRSExamDOCS")
            {

                rslt = MobileDataBase.GetValidUserDetails(tokenId, mUserid);
                rslt.Data = null;
            }
            {

                rslt.status = "0";
            }
            try
            {
                //  rslt.status = "0";
                if (rslt.status == "0")
                {
                    if (ImportLicenseDoc)//added newly // to differentiate if the upload is for Importer license details
                    {

                        LicenseNumber = Convert.ToString(System.Web.HttpContext.Current.Request.Form["LicenseNumber"]);
                        IssuanceDate = Convert.ToString(System.Web.HttpContext.Current.Request.Form["IssuanceDate"]);
                        ExpiryDate = Convert.ToString(System.Web.HttpContext.Current.Request.Form["ExpiryDate"]);
                        LicenseType = Convert.ToString(System.Web.HttpContext.Current.Request.Form["LicenseType"]);

                        string StatusCode = MobileDataBase.UniqueImporterLicenseCheck(LicenseNumber, OrgReqId, OrgId);
                        if (StatusCode == "-1")
                        {
                            rslt.status = "-11";
                            return new HttpResponseMessage()
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(rslt, Formatting.None)//Frdata
                           , System.Text.Encoding.UTF8, "application/json")
                            };
                        }
                    }

                    if (ConfigurationManager.AppSettings["UploadAccessImpersinationRequired"].ToString() == "Y")
                    {
                        using (UserImpersonation user = new UserImpersonation(UserId, Domain, Password))
                        {
                            if (user.ImpersonateValidUser())
                            {
                                UploadFile(rslt, DocumentName, DocumentType, OrgReqId, OrgId, ImportLicenseDoc, LicenseNumber, IssuanceDate, ExpiryDate, LicenseType);
                            }
                            else
                            {
                                throw new UnauthorizedAccessException("Access failed while uploading.");
                            }
                        }
                    }
                    else
                    {
                        UploadFile(rslt, DocumentName, DocumentType, OrgReqId, OrgId, ImportLicenseDoc, LicenseNumber, IssuanceDate, ExpiryDate, LicenseType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(rslt, Formatting.None)//Frdata
                , System.Text.Encoding.UTF8, "application/json")
            };
        }



        private MobileDataBase.Result UploadFile(MobileDataBase.Result rslt, string DocumentName, string DocumentType, string OrgReqId, string OrgId, bool ImportLicenseDoc, string LicenseNumber, string IssuanceDate, string ExpiryDate, string LicenseType)
        {
            MobileDataBase m = new MobileDataBase();
            string ID = m.GetNewIntKey("ScanRequestUploadDocs");

            string sPath = "";
            string UploadedFrom = Convert.ToString(System.Web.HttpContext.Current.Request.Form["UploadedFrom"]);

            string ShareFolderPath1 = Path.Combine(UploadedFrom, DateTime.Now.Year + "\\" + DateTime.Now.Month.ToString("00") + "\\" + DateTime.Now.Day.ToString("00") + '\\' + OrgReqId + '\\' + ID);// System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");


            sPath = Path.Combine(UploadedFrom, DateTime.Now.Year + "\\" + DateTime.Now.Month.ToString("00") + "\\" + DateTime.Now.Day.ToString("00") + '\\' + OrgReqId + '\\' + ID);// System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");
                                                                                                                                                                                    //  sPath = Path.Combine("Etrade\\" + UploadedFrom, DateTime.Now.Year + "\\" + DateTime.Now.Month.ToString("00") + "\\" + DateTime.Now.Day.ToString("00") + '\\' + OrgReqId + '\\' + ID);// System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");


            //   string ShareFolderPath1 = UploadedFrom + "\\" + year + "\\" + month + "\\" + day + "\\" + sProfileReferenceId;// +DeclarationId;



            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            FileResult Frdata = new FileResult();
            Frdata.DocumentName = DocumentName;
            Frdata.DocumentType = DocumentType;
            Frdata.OrgReqId = OrgReqId;
            Frdata.UploadedFrom = Convert.ToString(System.Web.HttpContext.Current.Request.Form["UploadedFrom"]);
            //  Frdata.NewFileName = DocumentName;
            Frdata.FilePath = DateTime.Now.Year + "\\" + DateTime.Now.Month.ToString("00") + "\\" + DateTime.Now.Day.ToString("00") + '\\' + OrgReqId;
            Frdata.IsUploaded = 'n';

            if (System.Web.HttpContext.Current.Request.Form["eservicerequestid"] != null)
            {
                Frdata.EserviceRequestId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]);

                if (!Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]).All(char.IsDigit))
                {
                    Frdata.EserviceRequestId = CommonFunctions.CsUploadDecrypt(Convert.ToString(System.Web.HttpContext.Current.Request.Form["eservicerequestid"]).ToString());
                }
            }
            // CHECK THE FILE COUNT.
            FileInfo F = null;
            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];

                if (hpf.ContentLength > 0)
                {

                    try
                    {

                        int iLen = hpf.ContentLength;
                        byte[] btArr = new byte[iLen];
                        hpf.InputStream.Read(btArr, 0, iLen);

                        var memoryStream = new MemoryStream(btArr);

                        Boolean isValidFile = false;

                        if (hpf.ContentType.Contains("image"))
                        {
                            isValidFile = IsImage(memoryStream);
                        }

                        else if (hpf.ContentType.Contains("application/pdf"))
                        {
                            isValidFile = IsPDF(memoryStream);
                        }


                        if (isValidFile)
                        {
                            if (!Directory.Exists(Path.Combine(ServerUploadFolder, ShareFolderPath1))) Directory.CreateDirectory(Path.Combine(ServerUploadFolder, ShareFolderPath1));
                            // sPath = ShareFolderPath1 + "\\" + FullfileName;
                            // file.SaveAs(Path.Combine(ServerUploadFolder, sPath));



                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                            //   if (!File.Exists(Path.Combine(sPath, Path.GetFileName(hpf.FileName))) && Regex.IsMatch(hpf.FileName.Trim(), "(\\.(jpg|jpeg|pdf))$", RegexOptions.IgnoreCase))
                            {
                                // SAVE THE FILES IN THE FOLDER.
                                string NewfileName;
                                string extension = Path.GetExtension(hpf.FileName);
                                if (UploadedFrom == "OrganizationRequests")
                                {
                                    NewfileName = DocumentName;//+ extension;
                                }
                                else
                                {
                                    NewfileName = DocumentName + extension;
                                }
                                NewfileName = NewfileName.Replace('/', '-');
                                Frdata.NewFileName = NewfileName;
                                //   hpf.SaveAs(Path.Combine(sPath, Path.GetFileName(hpf.FileName)));
                                hpf.SaveAs(Path.Combine(ServerUploadFolder, sPath, NewfileName));

                                Frdata.Name = Path.Combine(sPath, NewfileName);
                                Frdata.FilePath = Path.Combine(sPath, NewfileName);
                                //    F = new FileInfo(Path.Combine(sPath, Path.GetFileName(hpf.FileName)));
                                F = new FileInfo(Path.Combine(ServerUploadFolder, sPath, NewfileName));

                                Frdata.FileSize = (F.Length / 1024).ToString("0.00");
                                Frdata.IsUploaded = 'y';
                                break;

                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        using (System.IO.StreamWriter file =
              new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
                        {
                            file.WriteLine(ex.ToString());
                            file.Close();
                        }
                    }

                }
            }
            if (Frdata.IsUploaded == 'y')
            {
                rslt.Data = MobileDataBase.UpdateUploadDataDS(rslt.mUserId, Frdata, OrgId, ImportLicenseDoc, LicenseNumber, IssuanceDate, ExpiryDate, LicenseType);
                //  if (F != null)
                // {
                //     F.Rename(Frdata.NewFileName);
                //}
            }
            return rslt;
        }


        //private bool IsPDF(MemoryStream memoryStrem)//(String fileName)
        //{
        //    try
        //    {
        //        byte[] buffer = null;
        //        //FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        BinaryReader binaryReader = new BinaryReader(memoryStrem);
        //        //long numBytes = new FileInfo(fileName).Length;
        //        ////buffer = br.ReadBytes((int)numBytes);
        //        buffer = binaryReader.ReadBytes(5);

        //        var enc = new System.Text.ASCIIEncoding();
        //        var header = enc.GetString(buffer);

        //        //%PDF−1.0
        //        // If you are loading it into a long, this is (0x04034b50).
        //        if (buffer[0] == 0x25 && buffer[1] == 0x50
        //            && buffer[2] == 0x44 && buffer[3] == 0x46)
        //        {
        //            return header.StartsWith("%PDF-");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        using (System.IO.StreamWriter file =
        //      new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("~/logEmail.txt"), true))
        //        {
        //            file.WriteLine(ex.ToString());
        //            file.Close();
        //        }
        //    }

        //    return false;
        //}


        //private Boolean IsImage(MemoryStream memoryStream)
        //{
        //    Boolean isImage = false;

        //    System.Drawing.Image ff = System.Drawing.Image.FromStream(memoryStream);

        //    if (System.Drawing.Imaging.ImageFormat.Jpeg.Equals(ff.RawFormat) ||
        //        System.Drawing.Imaging.ImageFormat.Png.Equals(ff.RawFormat) ||
        //        System.Drawing.Imaging.ImageFormat.Gif.Equals(ff.RawFormat) ||
        //        System.Drawing.Imaging.ImageFormat.Bmp.Equals(ff.RawFormat) ||
        //        System.Drawing.Imaging.ImageFormat.Tiff.Equals(ff.RawFormat)
        //        )
        //    {
        //        isImage = true;
        //    }
        //    return isImage;
        //}

        //private string GetSingleOrDefault(IEnumerable<string> lst)
        //{
        //    if (lst == null) return "";
        //    else return lst.FirstOrDefault();
        //}

        
        [Route("OpenFile")]
        [HttpPost]
        public HttpResponseMessage OpenFile([FromBody] OpenDocumentParams data)
        {
            string login = ConfigurationManager.AppSettings["UploadAccessUserId"].ToString();
            string domain = ConfigurationManager.AppSettings["UploadAccessDomainName"].ToString();
            string password = Crypt.DecryptData(ConfigurationManager.AppSettings["UploadAccessPassword"].ToString());


            string tokenId = (data == null || data.tokenId == null) ? "" : data.tokenId;
            string mUserid = (data == null || data.mUserid == null) ? "" : data.mUserid;
            string DocumentId = (data == null || data.DocumentId == null) ? "" : data.DocumentId;

            //tokenId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tokenId"]);
            //mUserid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["mUserid"]);
            //DocumentId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentId"]);


            MobileDataBase.Result rslt = MobileDataBase.GetValidUserDetails(tokenId, mUserid);
            rslt.Data = null;

            try
            {
                if (rslt.status == "0")
                {
                    string SuffixPath = MobileDataBase.GetDocPath(DocumentId, mUserid);
                    string sPath = "";
                    sPath = Path.Combine(ServerUploadFolder, SuffixPath);
                    if (ConfigurationManager.AppSettings["UploadAccessImpersinationRequired"].ToString() == "Y")
                    {
                        using (UserImpersonation user = new UserImpersonation(login, domain, password))
                        {
                            if (user.ImpersonateValidUser())
                            {
                                return DownLoadFile(sPath);
                            }
                        }
                    }
                    else
                    {
                        return DownLoadFile(sPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
        }
        //azhar
        public HttpResponseMessage GetFile(string fileName)
        {
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Set the File Path.
            string filePath = fileName;

            //Check whether File exists.
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", fileName);
                throw new HttpResponseException(response);
            }

            //Read the File into a Byte Array.
            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.
            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            return response;
        }

        private HttpResponseMessage DownLoadFile(string sPath)
        {



            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(sPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);

                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                    httpResponseMessage.Content.Headers.Add("x-filename", Path.GetFileName(sPath));
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = Path.GetFileName(sPath);
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
            //if (!File.Exists(sPath))
            //    return new HttpResponseMessage(HttpStatusCode.BadRequest);

            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            ////response.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
            //Byte[] bytes = File.ReadAllBytes(sPath);
            ////String file = Convert.ToBase64String(bytes);
            //response.Content = new ByteArrayContent(bytes);
            //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            ////response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");// MimeTypeMap.GetMimeType(Path.GetFileName(sPath)));
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeTypeMap.GetMimeType(Path.GetFileName(sPath)));
            //response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(sPath);
            //response.StatusCode = HttpStatusCode.OK;
            //return response;
        }


      
        [Route("OpenFileForEservice")]
        [HttpPost]
        public HttpResponseMessage OpenFileForEservice([FromBody] OpenDocumentParams data)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(MobileDataBase.DownloadOrgReqDocForEservice(data), System.Text.Encoding.UTF8, "application/json")
            };
        }

        //        public HttpResponseMessage OpenFileForEservice([FromBody] OpenDocumentParams data)
        //        {
        //            string login = ConfigurationManager.AppSettings["UploadAccessUserId"].ToString();
        //            string domain = ConfigurationManager.AppSettings["UploadAccessDomainName"].ToString();
        //            string password = Crypt.DecryptData(ConfigurationManager.AppSettings["UploadAccessPassword"].ToString());


        //            string tokenId = (data == null || data.tokenId == null) ? "" : data.tokenId;
        //            string mUserid = (data == null || data.mUserid == null) ? "" : data.mUserid;
        //            string DocumentId = (data == null || data.DocumentId == null) ? "" : data.DocumentId;

        //            //tokenId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tokenId"]);
        //            //mUserid = Convert.ToString(System.Web.HttpContext.Current.Request.Form["mUserid"]);
        //            //DocumentId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["DocumentId"]);


        //            //MobileDataBase.Result rslt = MobileDataBase.GetValidUserDetails(tokenId, mUserid);
        //            //rslt.Data = null;
        //            MobileDataBase.Result rslt = new MobileDataBase.Result();
        //            rslt.status = "0";
        //            try
        //            {
        //                if (rslt.status == "0")
        //                {
        //                    string SuffixPath = MobileDataBase.GetDocPathForEservice(DocumentId,data.hiderefprofile,data.EserviceRequestid);

        //                    string sPath = "";
        //                    sPath = Path.Combine(ServerUploadFolder, SuffixPath);
        //                    if (ConfigurationManager.AppSettings["UploadAccessImpersinationRequired"].ToString() == "Y")
        //                    {
        //                        using (UserImpersonation user = new UserImpersonation(login, domain, password))
        //                        {
        //                            if (user.ImpersonateValidUser())
        //                            {
        ////                                return DownLoadFile(sPath);
        //                                return GetFile(sPath);

        //                               // GetFile
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return GetFile(sPath);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
        //        }

    }
    public static class FileInfoExtensions
    {
        /// <summary>
        /// behavior when new filename is exist.
        /// </summary>
        public enum FileExistBehavior
        {
            /// <summary>
            /// None: throw IOException "The destination file already exists."
            /// </summary>
            None = 0,
            /// <summary>
            /// Replace: replace the file in the destination.
            /// </summary>
            Replace = 1,
            /// <summary>
            /// Skip: skip this file.
            /// </summary>
            Skip = 2,
            /// <summary>
            /// Rename: rename the file. (like a window behavior)
            /// </summary>
            Rename = 3
        }
        /// <summary>
        /// Rename the file.
        /// </summary>
        /// <param name="fileInfo">the target file.</param>
        /// <param name="newFileName">new filename with extension.</param>
        /// <param name="fileExistBehavior">behavior when new filename is exist.</param>
        public static void Rename(this System.IO.FileInfo fileInfo, string newFileName, FileExistBehavior fileExistBehavior = FileExistBehavior.None)
        {
            string newFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(newFileName);
            string newFileNameExtension = System.IO.Path.GetExtension(newFileName);
            string newFilePath = System.IO.Path.Combine(fileInfo.Directory.FullName, newFileName);

            if (System.IO.File.Exists(newFilePath))
            {
                switch (fileExistBehavior)
                {
                    case FileExistBehavior.None:
                        throw new System.IO.IOException("The destination file already exists.");
                    case FileExistBehavior.Replace:
                        System.IO.File.Delete(newFilePath);
                        break;
                    case FileExistBehavior.Rename:
                        int dupplicate_count = 0;
                        string newFileNameWithDupplicateIndex;
                        string newFilePathWithDupplicateIndex;
                        do
                        {
                            dupplicate_count++;
                            newFileNameWithDupplicateIndex = newFileNameWithoutExtension + " (" + dupplicate_count + ")" + newFileNameExtension;
                            newFilePathWithDupplicateIndex = System.IO.Path.Combine(fileInfo.Directory.FullName, newFileNameWithDupplicateIndex);
                        } while (System.IO.File.Exists(newFilePathWithDupplicateIndex));
                        newFilePath = newFilePathWithDupplicateIndex;
                        break;
                    case FileExistBehavior.Skip:
                        return;
                }
            }
            System.IO.File.Move(fileInfo.FullName, newFilePath);
        }
    }

    public class ActionResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}

