using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Business;
using RS.Data;
using System.Web.UI;
using System.Runtime.InteropServices;

namespace RailwayReservationSystem.Controllers
{
    public class RailwayController : Controller
    {
        // GET: Railway

        RS.Business.RailwayBuss _railwaybus = new RailwayBuss();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
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
            var res = _railwaybus.GetDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);

            var result = new List<Train>(res.Count);
            foreach (var s in res)
            {
                // simple remapping adding extra info to found dataset
                result.Add(new Train
                {
                    Train_ID = s.Train_ID,
                    Train_name = s.Train_name,
                    Train_type = s.Train_type,
                    Source_stn = s.Source_stn,
                    Destination_stn = s.Destination_stn,
                    AvailableSeat = 200
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
            return Json(str, JsonRequestBehavior.AllowGet);
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




    }
}