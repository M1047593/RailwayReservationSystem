using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Business;
using RS.Data;
using System.Web.UI;
using System.Runtime.InteropServices;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using StackExchange.Redis;
using Newtonsoft.Json;


namespace RailwayReservationSystem.Controllers
{
    public class RailwayController : Controller
    {
        // GET: Railway
        public Lazy<ConnectionMultiplexer> connection;
        public IDatabase cache;
        public RailwayController()
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
                return ConnectionMultiplexer.Connect(cacheConnection);
            });

            cache = lazyConnection.Value.GetDatabase();
        }

        RS.Business.RailwayBuss _railwaybus = new RailwayBuss();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            //sendemail();
            return View();
        }

        public ActionResult Checkuser(string username, string password)
        {
            var data = _railwaybus.ValidateUser(username, password);
            return Json("Authenticated", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUser(RS.Data.RS_Registration Registration)
        {
            var data = _railwaybus.SaveUser(Registration);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult Station()
        {
            return View();
        }

        public ActionResult AddStation(int stationid, string stationname)
        {
            var flag = _railwaybus.AddStation(stationid, stationname);
            if (flag == true)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Failed", JsonRequestBehavior.AllowGet);

        }
        public ActionResult Train()
        {
            var stationlist = _railwaybus.GetStation();
            ViewBag.Station = new SelectList(stationlist, "Station_ID", "Station_Name");
            return View();
        }

        public ActionResult AddTrain(FormCollection s)
        {
            int txttrainid = Convert.ToInt32(s["TrainID"]);
            string txtTrainName = s["Trainname"];
            int Source_ID = Convert.ToInt32(s["Source_ID"]);
            int Destination_ID = Convert.ToInt32(s["Destination_ID"]);
            string ddltraintype = s["ddltraintype"];
            bool Monday = Convert.ToBoolean(s["Monday"].Split(',')[0]);
            bool Tuesday = Convert.ToBoolean(s["Tuesday"].Split(',')[0]);
            bool Wednesday = Convert.ToBoolean(s["Wednesday"].Split(',')[0]);
            bool Thursday = Convert.ToBoolean(s["Thursday"].Split(',')[0]);
            bool Friday = Convert.ToBoolean(s["Friday"].Split(',')[0]);
            string availabledays = Convert.ToString(Monday.ToString() + ',' + Tuesday.ToString() + ',' + Wednesday.ToString() + ',' + Thursday.ToString() + ',' + Friday.ToString());
            int fare1 = Convert.ToInt32(s["fare1"]);
            int fare2 = Convert.ToInt32(s["fare2"]);
            int fare3 = Convert.ToInt32(s["fare3"]);
            int totalseat = Convert.ToInt32(s["totalseat"]);
            int totalseat2 = Convert.ToInt32(s["totalseat2"]);
            int totalseat3 = Convert.ToInt32(s["totalseat3"]);
            bool flag = _railwaybus.SaveTrain(txttrainid, txtTrainName, Source_ID, Destination_ID, ddltraintype, availabledays, fare1, fare2, fare3, totalseat, totalseat2, totalseat3);
            if (flag == true)
                ViewBag.TheResult = true;
            cache.KeyDelete("gettrain");
            return RedirectToAction("ManageTrain");
        }
        public ActionResult Route()
        {
            var Trainlist = _railwaybus.GetTrain();
            var stationlist = _railwaybus.GetStation();
            ViewBag.Station = new SelectList(stationlist, "Station_ID", "Station_Name");
            ViewBag.Train = new SelectList(Trainlist, "Train_ID", "Train_name");
            return View();
        }

        public ActionResult AddRoute(FormCollection r)
        {
            int Train_ID = Convert.ToInt32(r["TrainID"]);
            int stopnumber = Convert.ToInt32(r["stopnumber"]);
            int stopstationid = Convert.ToInt32(r["stopstationid"]);
            string Arrivaltime = r["Arrivaltime"];
            string Departuretime = r["Departuretime"];
            int soucedistance = Convert.ToInt32(r["soucedistance"]);
            bool flag = _railwaybus.AddRoute(Train_ID, stopnumber, stopstationid, Arrivaltime, Departuretime, soucedistance);
            return RedirectToAction("Route");
        }

        public ActionResult FillTrainDetail(string Trainname)
        {
            var Trainlist = _railwaybus.GetTrainbyID(Trainname);
            return Json(Trainlist, JsonRequestBehavior.AllowGet);

        }

        public ActionResult FillSeatTrainDetail(string Trainname)
        {
            var stationlist = _railwaybus.GetTrainStation(Trainname);
            // var Trainlist = _railwaybus.GetTrainbyID(Trainname);
            return Json(stationlist, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ManageTrain()
        {
            var stationlist = _railwaybus.GetStation();
            ViewBag.Station = new SelectList(stationlist, "Station_ID", "Station_Name");
            return View();
        }

        public ActionResult GetTrainDetail(string Train_ID)
        {
            var Trainlist = _railwaybus.GetTrainbyIDdetail(Train_ID);
            return Json(Trainlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllTrain(DataTableAjaxPostModel model)
        {
            //var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            //{
            //    string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
            //    return ConnectionMultiplexer.Connect(cacheConnection);
            //});

            // Connection refers to a property that returns a ConnectionMultiplexer
            // as shown in the previous example.

            int filteredResultsCount;
            int totalResultsCount;
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;
            List<Train> myDeserializedObjList = new List<RS.Data.Train>();

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                // in this example we just default sort on the 1st column
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }
            if (!cache.StringGet("gettrain").HasValue)
            {
                var res = _railwaybus.GetDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);
                cache.StringSet("gettrain", JsonConvert.SerializeObject(res));
                myDeserializedObjList = (List<Train>)Newtonsoft.Json.JsonConvert.DeserializeObject(cache.StringGet("gettrain"), typeof(List<Train>));
            }
            else
                myDeserializedObjList = (List<Train>)Newtonsoft.Json.JsonConvert.DeserializeObject(cache.StringGet("gettrain"), typeof(List<Train>));
            var result = new List<Train>(myDeserializedObjList.Count);
            foreach (var s in myDeserializedObjList)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new Train
                {
                    Train_ID = s.Train_ID,
                    Train_name = s.Train_name,
                    Train_type = s.Train_type,
                    Source_stn = s.Source_stn,
                    Destination_stn = s.Destination_stn,
                   
                });
            };
            
            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = 20,
                recordsFiltered = 10,
                data = result
            });
        }


        public ActionResult BookTicket(int id, string Source_ID, string Destination_ID, string Jdate, string fare )
        {

            var stationlist = _railwaybus.GetStation(id);
            var sorcestationlist = stationlist.Where(x => x.Station_ID == Convert.ToInt32(Source_ID)).ToList();
            ViewBag.SourceStation = new SelectList(sorcestationlist, "Station_ID", "Station_Name");
            var Descstationlist = stationlist.Where(x => x.Station_ID == Convert.ToInt32(Destination_ID)).ToList();
            ViewBag.DestinationStation = new SelectList(Descstationlist, "Station_ID", "Station_Name");
            var Traindetail = _railwaybus.GetTrainbyID(id.ToString());
            ViewBag.Train = new SelectList(Traindetail, "Train_ID", "Train_name");
            ViewBag.TrainID = Traindetail[0].Train_ID;
            ViewBag.Fare = fare;
            ViewBag.Journeydate = Jdate;
            return View("BookTicket");
        }

        public ActionResult SearchTrain()
        {
            return View();
        }
        public ActionResult SearchTicketBook()
        {
            var stationlist = _railwaybus.GetStation();
            ViewBag.Station = new SelectList(stationlist, "Station_ID", "Station_Name");
            return View();
        }

        public ActionResult SearchTrainSearch(int Source_ID, int Destination_ID, string Jdate)
        {
            var Trainlist = _railwaybus.GetTrainBook(Source_ID, Destination_ID, Jdate);
            ViewBag.Jdate = Jdate;
            return Json(Trainlist, JsonRequestBehavior.AllowGet);

        }

        public ActionResult FillClassFare(string Trainname, string classid)
        {
            var classdetail = _railwaybus.classdetail(Trainname, classid);

            string Fare = classdetail[0].Fare_Class1.ToString();


            return Json(Fare, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewSchedule()
        {
            return View();
        }

        public ActionResult SearchViewSearch(string Trainnumber)
        {
            var Trainlist = _railwaybus.SearchViewSearch(Trainnumber);
            return Json(Trainlist, JsonRequestBehavior.AllowGet);

        }

        public ActionResult SeatAvailability()
        {
            var stationlist = _railwaybus.GetStation();

            ViewBag.Station = new SelectList(stationlist, "Station_ID", "Station_Name");
            var Traindetail = _railwaybus.GetTrainbyID("");
            ViewBag.Train = new SelectList(Traindetail, "Train_ID", "Train_name");
            return View();
        }

        public ActionResult Feedback()
        {
            return View();

        }

        public ActionResult CheckAvaliablity(int id, string Jdate , string seatclass)
        {
            string status = _railwaybus.CheckAvaliablity(id, Jdate, seatclass);
            
            string[] str = status.Split(',');
            if (status != "0")
            {
                ViewBag.Fare = str[1];
                return Json(str, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);

        }
   
        public ActionResult SaveBookticket(int TrainID,int ddlstation, int ddltrain, string Jdate, int Fare, string mnumber, string Name, int Age, string ddlgender, string ddlpreference ,string ddlproof,int Noofticket, string Name1, [Optional] int Age1, string ddlgender1, string ddlpreference1, string ddlproof1)
        {
            string flag = _railwaybus.SaveBookticket(TrainID, ddlstation, ddltrain, Jdate, Fare, mnumber, Name, Age, ddlgender, ddlpreference, ddlproof, Noofticket, Name1, Age1, ddlgender1, ddlpreference1, ddlproof1);
            string[] str = flag.Split(',');
            if (flag != "0")
            {
                sendemail();
               return Json(str, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
           
        }


        public JsonResult GetBookedTicket(DataTableAjaxPostModel model)
        {
            // action inside a standard controller
            int filteredResultsCount;
            int totalResultsCount;
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {
                // in this example we just default sort on the 1st column
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }
            var res = _railwaybus.GetBookedDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);

            var result = new List<Passenger>(res.Count);
            foreach (var s in res)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new Passenger
                {
                    Train_ID = s.Train_ID,
                    PNR = s.PNR,
                    ReservationStatus = s.ReservationStatus,
                    Source = s.Source,
                    Destination = s.Destination,
                    JourneyDate = s.JourneyDate
                });
            };

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = result
            });
        }

        public ActionResult TransactionHistory()
        {
            return View();

        }

        public void sendemail()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("kannan.eee89@gmail.com", "Kannan"),
                Subject = "Booking Confirmation on IRCTC, Train: 16796, 25-Aug-2018, SL, TJ - MS",
                PlainTextContent = "Hello, Email!",
                HtmlContent = @"
<html>
<head>
<meta charset=""utf-8"" />
<title></title>
</head>
<body>
    <table style=""width:60%;margin-left: 10%;"">
        <tr>
            <td>
                Train ID : 
            </td>
            <td >
                Train Name :
            </td>
        </tr>
        <tr>
            <td>
                Boarding At  :
            </td>
            <td>
                Reservation Up to :	
            </td>
        </tr>
        <tr>
            <td>
                PNR Number :
            </td>
            <td>
                Class :
            </td>
        </tr>
        <tr>
            <td>
                Date of Booking :
            </td>
            <td>
                Date of Journey :
            </td>
        </tr>
        <tr>
            <td>
                Amount :
            </td>
            <td>
                Scheduled Departure* :
            </td>
        </tr>
    </table>



    <div class=""col-md-12"" style=""font-weight:200"">
        <div class=""form-group"">
            <table class=""table table-sm fz12"" style=""width:60%;margin-top: 50px;margin-left: 10%;"">
                <thead class=""text-uppercase"">
                    <tr>
                        <th scope = ""col"" style=""width: 400px;""><strong>Passenger Name</strong></th>
                        <th scope = ""col"" style= ""width: 520px;""><strong>Gender</strong></th>                        
                        <th scope = ""col"" style=""width: 520px;""><strong>Age</strong> </th>
                        <th scope = ""col"" style=""width: 520px;""><strong>Seat Number</strong> </th>
                        <th scope = ""col"" style= ""width: 520px;""><strong>Seat Location</strong> </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td style = ""text-align: center;"">
                            Name :
                        </td>
                        <td style = ""text-align: center;"">
                            Age:
                        </td>
                        <td style = ""text-align: center;"">
                            Name :
                        </td>
                        <td style = ""text-align: center;"">
                            Age:
                        </td>
                        <td style = ""text-align: center;"">
                            Name :
                        </td>
                        
                    </tr>
                    <tr>
                        <td style = ""text-align: center;"">
                            Name :
                        </td>
                        <td style = ""text-align: center;"">
                            Age:
                        </td>
                        <td style = ""text-align: center;"">
                            Name :
                        </td>
                        <td style = ""text-align: center;"">
                            Age:
                        </td>
                        <td style = ""text-align: center;"">
                            Name :
                        </td>

                    </tr>

                </tbody>
            </table>

        </div>
    </div>

</body>
</html>"
            };
            msg.AddTo(new EmailAddress("Kannan.eee89@gmail.com", "Admin"));
            var response = client.SendEmailAsync(msg);


        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    CloudStorageAccount cloudStorageAccount = GetConnectionString();
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("reservationsystemcontainer");

                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;
                        CloudBlockBlob azureBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
                        azureBlockBlob.UploadFromStream(Request.Files[i].InputStream);


                        //// Checking for Internet Explorer  
                        //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        //{
                        //    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        //    fname = testfiles[testfiles.Length - 1];
                        //}
                        //else
                        //{
                        //    fname = file.FileName;
                        //}

                        //// Get the complete folder path and store the file inside it.  
                        //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        //file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }


        //public static void UploadSupportingDocuments(string filename)
        //{
        //    long id = Convert.ToInt64(Request.Form["ID"]);
        //    if (Request.Files.Count > 0)
        //    {
        //        CloudStorageAccount cloudStorageAccount = GetConnectionString();
        //        CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //        CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);

        //        //for (int fileNum = 0; fileNum < Request.Files.Count; fileNum++)
        //        //{
        //            string fileName = filename;//Path.GetFileName(Request.Files[fileNum].FileName);
        //          //  if (Request.Files[fileNum] != null &&
        //            //Request.Files[fileNum].ContentLength > 0)
        //            //{
        //                CloudBlockBlob azureBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
        //                azureBlockBlob.UploadFromStream(Request.Files[fileNum].InputStream);
        //                //SupportingDocument detail = new SupportingDocument();
        //                //detail.TempPolicyNo = id;
        //                //detail.FileName = fileName;
        //                //detail.CreatedBy = (int)Session["UserId"];
        //                //if (fileNum == 0)
        //                //    detail.DisplayFileName = "Aadhar Document";
        //                //else if (fileNum == 1)
        //                //    detail.DisplayFileName = "Pan Document";
        //                //else if (fileNum == 2)
        //                //    detail.DisplayFileName = "Medical Checkup Document";
        //                //manager.InsertSupportingDocument(detail);
        //            }
        //            fileNum += 1;
        //        }
        //    }
        //    //List<SupportingDocument> listDocument = manager.GetSupportingDocument(id);
        //    //return Json(listDocument, JsonRequestBehavior.AllowGet);

        //}

        public static CloudStorageAccount GetConnectionString()
        {
            string accountname = "301chennai";// ConfigurationManager.AppSettings["accountName"];
            string key = "5v4AEFp2B/1RsqauelgHm79O8Ml27Vi0IT8FbML7Ao0NEDEIUnI7SvJIAEABnDPUr+k2e1DpLCPK1NVKlHG0SQ==";// ConfigurationManager.AppSettings["key"];
            string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountname, key);
            return CloudStorageAccount.Parse(connectionString);
        }

        public FileResult DownloadSupportingDocument(string fileName)
        {
            CloudStorageAccount cloudStorageAccount = GetConnectionString();
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("reservationsystemcontainer");
            CloudBlockBlob azureBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
            Stream blobStream = azureBlockBlob.OpenRead();
            return File(blobStream, azureBlockBlob.Properties.ContentType, fileName);
        }








    }
}