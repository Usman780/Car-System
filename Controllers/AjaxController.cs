using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarSystem.Models;
using CarSystem.BL;
using CarSystem.Helping_Classes;

namespace CarSystem.Controllers
{
    public class AjaxController : Controller
    {
        private GeneralPurpose gp = new GeneralPurpose();
        private DatabaseEntities de = new DatabaseEntities();

        #region User Starting

        [HttpPost]
        public ActionResult GetUserDataTableList(string name = "", string email = ""/*, int gender = -1*/)  //  change   2/18/2022
        {
            #region for custom search

            List<User> ulist = new UserBL().GetActiveUsersList(de).Where(x => x.Role == 2).ToList();
            
            if (name != "")
            {
                ulist = ulist.Where(x => x.Name.ToLower().Contains(name.Trim().ToLower())).ToList();
            }
            if (email != "")
            {
                ulist = ulist.Where(x => x.Email.ToLower().Contains(email.Trim().ToLower())).ToList();
            }
            //if (gender != -1)
            //{
            //    ulist = ulist.Where(x => x.Gender == gender).ToList();        change   2/18/2022
            //}

            #endregion custom search

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (!String.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        ulist = ulist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        ulist = ulist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = ulist.Count();
            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                ulist = ulist.Where(x => x.Contact != null && x.Contact.Trim().ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.Name != null && x.Name.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.Email != null && x.Email.ToLower().Contains(searchValue.Trim().ToLower())
                                    ).ToList();
            }
            int totalrowsafterfilterinig = ulist.Count();
            // pagination
            ulist = ulist.Skip(start).Take(length).ToList();

            //string gen = "";                                      change   2/18/2022

            List<UserDTO> udto = new List<UserDTO>();
            foreach (User item in ulist)
            {

                //if (item.Gender == 2)
                //    gen = "Female";                               change   2/18/2022
                //else
                //    gen = "Male";

                UserDTO obj = new UserDTO()
                {
                    Id = item.Id,
                    EncId = StringCipher.EncryptId(item.Id),
                    Name = item.Name,
                    Email = item.Email,
                    Contact = item.Contact,
                    //Gender = gen                                       change   2 / 18 / 2022
                    Gender="1"
                };

                udto.Add(obj);
            }

            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetUserById(int id)
        {
            //string gender = "";                                        change   2/18/2022
            User user = new UserBL().GetActiveUserById(id, de);
            if (user == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            //if (user.Gender == 2)
            //    gender = "Female";                                     change   2/18/2022
            //else
            //    gender = "Male";

            UserDTO obj = new UserDTO()
            {
                Id = user.Id,
                EncId = StringCipher.EncryptId(user.Id),
                Name = user.Name,
                Email = user.Email,
                Contact = user.Contact,
                //Gender = gender,                                        change   2/18/2022
                Gender = "1",
                Password = StringCipher.Decrypt(user.Password)
                //ProfilePath = u.ProfilePath,
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion User Ending
                
        #region Car Detail Starting

        [HttpPost]
        public ActionResult GetCarDetailDataTableList(string lotyear="", string lotmodel="", string damagetype="")  
        {

            #region for custom search
            
            List<CarDetail> cdlist = new CarDetailBL().GetActiveCarDetailList(de).ToList();
                       
                        
            if (lotyear != "")
            {
                cdlist = cdlist.Where(x => x.LotYear.ToLower().Contains(lotyear.ToLower())).ToList();
            }
            if (lotmodel != "")
            {
                cdlist = cdlist.Where(x => x.LotModel.ToLower().Contains(lotmodel.ToLower())).ToList();
            }
            if (damagetype != "")
            {
                cdlist = cdlist.Where(x => x.DamageType.ToLower().Contains(damagetype.ToLower())).ToList();
            }

            #endregion custom search



            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (!String.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        cdlist = cdlist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        cdlist = cdlist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = cdlist.Count();
            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                cdlist = cdlist.Where(x => x.LotYear != null && x.LotYear.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.LotMake != null && x.LotMake.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.LotModel != null && x.LotModel.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.LotRunCondition != null && x.LotRunCondition.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.DamageTypeDescription != null && x.DamageTypeDescription.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.CopartFacilityName != null && x.CopartFacilityName.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.SaleTitleState != null && x.SaleTitleState.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.SaleTitleType != null && x.SaleTitleType.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.DamageType != null && x.DamageType.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.LotColor != null && x.LotColor.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.HasKey != null && x.HasKey.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.OdometerReading != null && x.OdometerReading.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.SalePrice != null && x.SalePrice.ToLower().Contains(searchValue.Trim().ToLower()) ||
                                    x.RepairCost != null && x.RepairCost.ToLower().Contains(searchValue.Trim().ToLower()) 
                                    ).ToList();
            }
            int totalrowsafterfilterinig = cdlist.Count();
            // pagination
            cdlist = cdlist.Skip(start).Take(length).ToList();


            List<CarDetail> cd_Dto = new List<CarDetail>();
            foreach (CarDetail item in cdlist)
            {

                CarDetail obj = new CarDetail()
                {
                    Id = item.Id,
                   LotYear = item.LotYear,
                   LotMake = item.LotMake,
                   LotModel = item.LotModel,
                   LotRunCondition = item.LotRunCondition,
                   DamageTypeDescription = item.DamageTypeDescription,
                   CopartFacilityName = item.CopartFacilityName,
                   SaleTitleState = item.SaleTitleState,
                   SaleTitleType = item.SaleTitleType,
                   DamageType = item.DamageType,
                   LotColor = item.LotColor,
                   HasKey = item.HasKey,
                   OdometerReading = item.OdometerReading,
                   SalePrice = item.SalePrice,
                   RepairCost = item.RepairCost
            };

                cd_Dto.Add(obj);
            }

            return Json(new { data = cd_Dto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCarDetailById(int id)
        {
            CarDetail cd = new CarDetailBL().GetActiveCarDetailById(id, de);
            if (cd == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }


            //CarDetailDTO obj = new CarDetailDTO()
            CarDetail obj = new CarDetail()
            {
                Id = cd.Id,
                LotYear = cd.LotYear,
                LotMake = cd.LotMake,
                LotModel = cd.LotModel,
                LotRunCondition = cd.LotRunCondition,
                DamageTypeDescription = cd.DamageTypeDescription,
                CopartFacilityName = cd.CopartFacilityName,
                SaleTitleState = cd.SaleTitleState,
                SaleTitleType = cd.SaleTitleType,
                DamageType = cd.DamageType,
                LotColor = cd.LotColor,
                HasKey = cd.HasKey,
                OdometerReading = cd.OdometerReading,
                SalePrice = cd.SalePrice,
                RepairCost = cd.RepairCost
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion Car Detail Ending


        #region Partail Views => form1
        [HttpPost]

        public ActionResult GetLotList(string lotYear="", string lotMake ="" ,  string lotModel="")                             //int subjectId
        {

            if (!string.IsNullOrEmpty(lotYear)  &&  string.IsNullOrEmpty(lotMake) && string.IsNullOrEmpty(lotModel))
            {
                //ViewBag.lotYearVal = lotYear;

                List<string> CDlist = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotYear.ToLower() == lotYear.ToLower()).Select(x => x.LotMake).Distinct().ToList();

                return Json(CDlist, JsonRequestBehavior.AllowGet);
            }

            else if(!string.IsNullOrEmpty(lotYear) && !string.IsNullOrEmpty(lotMake) && string.IsNullOrEmpty(lotModel))
            {
                //ViewBag.lotMakeVal = lotMake;

                List<string> CDlist = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotYear.ToLower() == lotYear.ToLower() && x.LotMake.ToLower() == lotMake.ToLower()).Select(x => x.LotModel).Distinct().ToList();

                return Json(CDlist, JsonRequestBehavior.AllowGet);
            }

            else if (!string.IsNullOrEmpty(lotYear) && !string.IsNullOrEmpty(lotMake) && !string.IsNullOrEmpty(lotModel))
            {
                ViewBag.lotModelVal = lotModel;

                List<string> CDlist = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotYear.ToLower() == lotYear.ToLower() && x.LotMake.ToLower() == lotMake.ToLower() && x.LotModel.ToLower() == lotModel.ToLower()).Select(x => x.LotModel).Distinct().ToList();

                return Json(CDlist, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        #endregion

        [HttpPost]
        public ActionResult SubmitFullForm(string LotYear="", string LotMake="", string LotModel="", 
                                            int ZipCode = -1, string BodyStyleOfCar = "",
                                           string DriveWheel = "", string EngineSize = "", 
                                           string Transmission = "", string FuelType = "",
                                           string OdometerReading = "" , string[] PrimaryDamage = null, 
                                           string SalePrice = "", string TitleType = "") 
        {
            if (string.IsNullOrEmpty(LotYear))
            {
                return Json("Year is Required", JsonRequestBehavior.AllowGet);
            }
            List<CarDetail> cda = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotModel == "SANTA FE").ToList();
            if (!string.IsNullOrEmpty(LotYear) && !string.IsNullOrEmpty(LotMake) && !string.IsNullOrEmpty(LotModel))
            {

                CarDetail getObj = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotYear.ToLower() == LotYear.ToLower()
                                    && x.LotMake.ToLower() == LotMake.ToLower() && x.LotModel.ToLower() == LotModel.ToLower()).FirstOrDefault();

                int count1 = 0;

                if (getObj == null)
                {

                    if (count1 == 0)
                    {
                        List<string> lista = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotMake.ToLower() == LotMake.ToLower())
                            .Select(x => x.LotYear).Distinct().ToList();
                        List<int> myLotYearIntList = lista.Select(s => int.Parse(s)).ToList();

                        var number = Convert.ToInt32(LotYear);

                        // find closest to number

                        int closestYear = myLotYearIntList.OrderBy(item => Math.Abs(number - item)).First();

                        string year = Convert.ToString(closestYear);

                        CarDetail getObj1 = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotYear.ToLower() == year.ToLower()
                                        && x.LotMake.ToLower() == LotMake.ToLower() && x.LotModel.ToLower() == LotModel.ToLower()).FirstOrDefault();
                        if (getObj != null)
                        {
                            string name1 = getObj1.LotYear + " " + getObj1.LotMake + " " + getObj1.LotModel;

                            double GetSalePrice1 = Math.Round((Convert.ToDouble(getObj1.SalePrice) / 100) * 65, 2);

                            CarDetailDisplayNameAndPriceDTO cdDTO1 = new CarDetailDisplayNameAndPriceDTO()
                            {
                                Name = name1,
                                GetSalePrice = GetSalePrice1
                            };

                            return Json(cdDTO1, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            count1++;
                        }


                        if (count1 > 0)
                        {
                            List<string> modelList1 = (List<string>)new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotMake.ToLower() == LotMake.ToLower()
                            && x.LotYear == year).Select(x => x.LotModel).Distinct().ToList();

                            for (int w = 0; w <= modelList1.Count -1; w++)
                            {
                                string model = modelList1[w];

                                CarDetail getObj2 = new CarDetailBL().GetActiveCarDetailList(de).Where(x => x.LotYear.ToLower() == year.ToLower()
                                            && x.LotMake.ToLower() == LotMake.ToLower() && x.LotModel.ToLower() == model.ToLower() &&
                                            x.SalePrice == SalePrice).FirstOrDefault();

                                if (getObj2 != null)
                                {
                                    string name2 = getObj2.LotYear + " " + getObj2.LotMake + " " + getObj2.LotModel;

                                    double GetSalePrice2 = Math.Round((Convert.ToDouble(getObj1.SalePrice) / 100) * 65, 2);

                                    CarDetailDisplayNameAndPriceDTO cdDTO2 = new CarDetailDisplayNameAndPriceDTO()
                                    {
                                        Name = name2,
                                        GetSalePrice = GetSalePrice2
                                    };

                                    return Json(cdDTO2, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    continue;
                                }
                            }


                        }


                    }


                    CarDetailDisplayNameAndPriceDTO cdDTO3 = new CarDetailDisplayNameAndPriceDTO()
                    {
                        Name = "Not Found",
                        GetSalePrice = 0.00
                    };
                    return Json(cdDTO3, JsonRequestBehavior.AllowGet);






















                    //CarDetailDisplayNameAndPriceDTO cdDTO1 = new CarDetailDisplayNameAndPriceDTO()
                    //{
                    //   Name = "Not Found",
                    //   GetSalePrice = 0.00
                    //};
                    //return Json(cdDTO1, JsonRequestBehavior.AllowGet);
                }

                string name = getObj.LotYear + " " + getObj.LotMake + " " + getObj.LotModel;

                double GetSalePrice = Math.Round((Convert.ToDouble(getObj.SalePrice) / 100) * 65, 2);

                CarDetailDisplayNameAndPriceDTO cdDTO = new CarDetailDisplayNameAndPriceDTO()
                {
                    Name = name,
                    GetSalePrice = GetSalePrice
                };

                return Json(cdDTO, JsonRequestBehavior.AllowGet);


            }
            

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateEmail(string email, int id = -1)
        {
            return Json(gp.ValidateEmail(email,id), JsonRequestBehavior.AllowGet);
        }

    }
}