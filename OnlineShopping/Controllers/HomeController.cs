using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace FinalProject01.Controllers
{
    public class HomeController : Controller
    {
        string connDB = WebConfigurationManager.ConnectionStrings["connDB"].ConnectionString;


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ProductEntry()
        {
            return View();
        }

        public ActionResult ListAllProducts()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult EntryProduct()
        {
            return View();
        }

        public ActionResult Mycart()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductEntry(FormCollection collection, HttpPostedFileBase uploadImg)
        {
            var itmcde = Convert.ToString(collection["txtCode"]);
            var itmname = Request["txtname"];
            var itmdesc = Request["txtDesc"];
            var itmprce = Request["txtprice"];
            var itmonhand = Request["txtonhand"];
            var date = Request["datepicker"];
            var category = Request["Category"];

            if (uploadImg != null)
            {
                try
                {
                    string filename = Path.GetFileName(uploadImg.FileName);
                    var checkextension = Path.GetExtension(uploadImg.FileName).ToUpper();
                    int filesize = uploadImg.ContentLength;
                    string logPath = "C:\\Uploads";
                    string filepath = Path.Combine(logPath, filename);
                    uploadImg.SaveAs(filepath);
                    using (var db = new SqlConnection(connDB))
                    {
                        db.Open();
                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "INSERT INTO ITMTBL (ITMNUM,ITMNAME,ITMDESC,ITMIMG,ITMONHAND,ITMPRICE,ITMDATE, ITMCATEGORY)"
                                + " VALUES ("
                                + " @NUM,"
                                + " @NAME,"
                                + " @DESC,"
                                + " @IMG,"
                                + " @ONHAND,"
                                + " @PRICE,"
                                + " @DATE,"
                                + " @ITCATEGORY)";
                            cmd.Parameters.AddWithValue("@NUM", itmcde);
                            cmd.Parameters.AddWithValue("@NAME", itmname);
                            cmd.Parameters.AddWithValue("@DESC", itmdesc);
                            cmd.Parameters.AddWithValue("@IMG", filename);
                            cmd.Parameters.AddWithValue("@ONHAND", itmonhand);
                            cmd.Parameters.AddWithValue("@PRICE", itmprce);
                            cmd.Parameters.AddWithValue("@DATE", date);
                            cmd.Parameters.AddWithValue("@ITCATEGORY", category);
                            var ctr = cmd.ExecuteNonQuery();
                            if (ctr >= 1)
                            {
                                Response.Write("<script>alert('Data Save')</script>");
                            }
                            else
                                Response.Write("<script>alert('Data Not Save')</script>");
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }

            return View();
        }

        public ActionResult getItemCode()
        {
            var data = new List<object>();
            var itmcode = "";
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT MAX(ID) as 'MAXID' FROM ITMTBL";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new
                        {
                            itmcode = reader["MAXID"].ToString(),
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult Image(string filename)
        {
            var folder = "";
            var filepath = "";

            try
            {
                folder = "C:\\Uploads";
                filepath = Path.Combine(folder, filename);
                if (!System.IO.File.Exists(filepath))
                {
                    //image not found here
                }
            }
            catch (Exception)
            {
                //throw;
            }

            var mime = System.Web.MimeMapping.GetMimeMapping(Path.GetFileName(filepath));
            Response.Headers.Add("Content-Disposition", "Inline");

            return new FilePathResult(filepath, mime);
        }

        public ActionResult Cart1()
        {
            var data = new List<object>();
            string email = Session["email"].ToString();
            var itmcode = Request["itmnum"];
            var qty = Convert.ToInt32(Request["qty"]);
            var onhand = 0;
            var date = DateTime.Now;
            var price = Request["itemprice"].Trim();
            var itmimg = "";
            var name = Request["name"].Trim();

            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd1 = db.CreateCommand())
                {
                    cmd1.CommandType = CommandType.Text;
                    cmd1.CommandText = "SELECT * FROM ITMTBL WHERE ITMNUM ='" + itmcode + "'";
                    SqlDataReader reader = cmd1.ExecuteReader();

                    if (reader.Read())
                    {
                        itmimg = reader["ITMIMG"].ToString();

                        onhand = Convert.ToInt32(reader["ITMONHAND"]);

                    }
                }
                db.Close();

            }

            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    double newOnhand = onhand - qty;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM ORDERTBL WHERE ITMNO ='" + itmcode + "'";
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int iQuantity = Convert.ToInt32(reader["ITMQTY"]);
                        int fQuantity = iQuantity + qty;

                        if (fQuantity <= onhand)
                        {

                            using (var db1 = new SqlConnection(connDB))
                            {
                                db1.Open();
                                using (var cmd1 = db1.CreateCommand())
                                {
                                    cmd1.CommandType = CommandType.Text;
                                    cmd1.CommandText = "UPDATE ORDERTBL SET ITMQTY = '" + fQuantity + "' WHERE ITMNO = '" + itmcode + "' ";
                                    var ctr = cmd1.ExecuteNonQuery();

                                    if (ctr >= 1)
                                    {
                                        data.Add(new
                                        {
                                            exist = true
                                        });
                                    }
                                }
                            }

                        }
                        else
                        {
                            data.Add(new
                            {
                                exist = false
                            });
                        }


                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        using (var db2 = new SqlConnection(connDB))
                        {
                            db2.Open();
                            using (var cmd2 = db2.CreateCommand())
                            {

                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "INSERT INTO ORDERTBL (ITMNO, ITMQTY, ITMPRICE, DTADDED, ITMNAME, ITMIMG, EMAIL) "
                                    + "VALUES ("
                                    + " @ITMNO,"
                                    + " @ITMQTY,"
                                    + " @ITMPRICE,"
                                    + " @DTADDED,"
                                    + " @NAME,"
                                    + " @ITMIMG,"
                                    + " @MAIL)";

                                cmd.Parameters.AddWithValue("@ITMNO", itmcode);
                                cmd.Parameters.AddWithValue("@ITMQTY", qty);
                                cmd.Parameters.AddWithValue("@ITMPRICE", price);
                                cmd.Parameters.AddWithValue("@DTADDED", date);
                                cmd.Parameters.AddWithValue("@NAME", name);
                                cmd.Parameters.AddWithValue("@ITMIMG", itmimg);
                                cmd.Parameters.AddWithValue("@MAIL", email);
                                var ctr = cmd.ExecuteNonQuery();
                                if (ctr > 0)
                                {

                                    data.Add(new
                                    {
                                        added = true,
                                        prc = price,
                                        code = itmcode,
                                        qtyx = qty,
                                        dat = date

                                    });
                                }
                                else
                                {
                                    Response.Write("<script>alert('Cant add!')</script>");
                                }
                            }
                        }

                    }
                }
            }



            return Json(data, JsonRequestBehavior.AllowGet);
        }
              

        [HttpPost]
        public ActionResult Registration(FormCollection collection)
        {
            var lastname = Request["txtLastname"];
            var firstname = Request["txtFirstname"];
            var address = Request["txtAddress"];
            var bdate = Request["txtBdate"];
            var contactnum = Request["txtContact"];
            var email = Request["txtEmail"];
            var username = Request["txtUsername"];
            var password = Request["txtPassword"];
            var role = Request["rdRole"];


            try
            {
                using (var db = new SqlConnection(connDB))
                {
                    db.Open();
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM USERTBL WHERE USERNAME = '" + username + "' OR EMAIL = '" + email + "' ";
                        SqlDataReader rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            Response.Write("<script>alert('This email is already registered.!')</script>");
                            rd.Close();
                        }
                        else
                        {
                            double bal = 0.00;
                            rd.Close();
                            cmd.CommandText = "INSERT INTO USERTBL (LASTNAME,FIRSTNAME,ADDRESS,BIRTHDATE,CONTACTNUM,EMAIL,USERNAME, PASSWORD, MONEY, ROLE)"
                                + " VALUES ("
                                + " @LNAME,"
                                + " @FNAME,"
                                + " @ADDRESS,"
                                + " @BDATE,"
                                + " @CONTACT,"
                                + " @EMAIL,"
                                + " @USERNAME, "
                                + " @PSWD,"
                                + " @AMT,"
                                + " @ROLE)";
                            cmd.Parameters.AddWithValue("@LNAME", lastname);
                            cmd.Parameters.AddWithValue("@FNAME", firstname);
                            cmd.Parameters.AddWithValue("@ADDRESS", address);
                            cmd.Parameters.AddWithValue("@BDATE", bdate);
                            cmd.Parameters.AddWithValue("@CONTACT", contactnum);
                            cmd.Parameters.AddWithValue("@EMAIL", email);
                            cmd.Parameters.AddWithValue("@USERNAME", username);
                            cmd.Parameters.AddWithValue("@PSWD", password);
                            cmd.Parameters.AddWithValue("@AMT", bal);
                            cmd.Parameters.AddWithValue("@ROLE", role);
                            var ctr = cmd.ExecuteNonQuery();

                            if (ctr > 0)
                            {
                                Response.Write("<script>alert('Registered Successfully!')</script>");
                                Response.Redirect("LogIn");
                            }
                            else
                                Response.Write("<script>alert('Cannot create your account. ')</script>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Sorry, something went wrong.')</script>");
                Response.Write(ex.Message);
            }

            return View();
        }

        public ActionResult LogIn()
        {
            if (Session["email"] != null)
            {
                return RedirectToAction("Registration", "Home", new { email = Session["email"].ToString() });
            }
            else
            {
                return View();

            }

        }

        [HttpPost]
        public ActionResult LogIn(FormCollection collection)
        {
            var user = Request["txtUsername"];
            var password = Request["txtPassword"];

            try
            {
                if(user == "Admin" && password =="Admin")
                {
                    Response.Redirect("AdminView");
                }
                else
                {
                    using (var db = new SqlConnection(connDB))
                    {
                        db.Open();
                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = "SELECT * FROM USERTBL  WHERE USERNAME = '" + user + "' AND PASSWORD = '" + password + "' ";
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                Session["email"] = reader["EMAIL"].ToString();
                                Response.Redirect("ListAllProducts");
                            }
                            else
                            {
                                Response.Write("<script>alert('Invalid Credentials!')</script>");
                            }
                        }

                    }
                }
               
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Something went wrong...')</script>");
                Response.Write(ex);
            }
            return View();
        }

        //for payment method
        public ActionResult Payment()
        {
            return View();

        }
        public ActionResult AdminView()
        {
            return View();

        }

        public ActionResult Pay()
        {
            string email = Session["email"].ToString();
            var arlist1 = new ArrayList();
            var db = new SqlConnection(connDB);
            bool updated;
            //get the updated onhand item by joining the odertbl and itmtbl
            db.Open();
            var cmd1 = db.CreateCommand();

            cmd1.CommandType = CommandType.Text;
            cmd1.CommandText = "SELECT ITMQTY, ITMONHAND, ITMNUM  FROM ORDERTBL, ITMTBL WHERE ORDERTBL.ITMNO = ITMTBL.ITMNUM ";
            var reader = cmd1.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {


                    var itm = reader["ITMNUM"];
                    string itmnum = itm.ToString();
                    int onhand = Convert.ToInt32(reader["ITMONHAND"]);
                    int cartqty = Convert.ToInt32(reader["ITMQTY"]);
                    int currOnhand = onhand - cartqty;

                    var arlist2 = new ArrayList()
                            {
                                itm, currOnhand
                            };

                    arlist1.AddRange(arlist2);


                }
            }


            for (int i = 0; i < arlist1.Count; i++)
            {

                if (i % 2 == 0)
                {
                    var itmno = arlist1[i];
                    var currOnhand = arlist1[i + 1];
                    //update the onhand item of the itemtable
                    using (var db3 = new SqlConnection(connDB))
                    {
                        db3.Open();
                        using (var cmd3 = db3.CreateCommand())
                        {
                            cmd3.CommandType = CommandType.Text;
                            cmd3.CommandText = "UPDATE ITMTBL SET ITMONHAND = '" + currOnhand + "' WHERE ITMNUM = '" + itmno + "' ";
                            SqlDataReader rdr = cmd3.ExecuteReader();
                            if (rdr.Read())
                            {
                                var updatedOnHand = rdr["ITMONHAND"];
                            }
                        }
                    }

                }
                 updated = true;
            }
            //will delete the item to the odertable with the specific email or user
            bool deleted = false;
            using (var db4 = new SqlConnection(connDB))
            {
                db4.Open();
                using (var cmd = db4.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM ORDERTBL WHERE EMAIL='"+email+"'";
                    SqlDataReader reader3 = cmd.ExecuteReader();
                    if (reader3.Read())
                    {
                        //unsucssesful deleting the item
                    }
                    else
                    {
                        deleted = true;
                    }

                }
            }

            return Json(deleted, JsonRequestBehavior.AllowGet);

        }

        //fuction to deposit the money in the user ewallet
        public ActionResult Deposit()
        {
            var data = new List<object>();
            var depoAmt = Request["amount"];
            string email = Session["email"].ToString();
            var balance = Request["balance"];
            double currBalance = 0.00;
            try
            {
                currBalance = Convert.ToDouble(balance) + Convert.ToDouble(depoAmt);
                using (var db = new SqlConnection(connDB))
                {
                    db.Open();
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE USERTBL SET MONEY='" + currBalance + "' WHERE EMAIL='" + email + "'";
                        var ctr = cmd.ExecuteNonQuery();
                        if (ctr >= 1)
                        {
                            Response.Write("<script>alert('Transaction Completed...') </ script >");

                        }
                        else
                        {
                            Response.Write("<script>alert('Plaese try again...') </ script >");
                        }
                    }
            }
            }
            catch(Exception)
            {
                Response.Write("<script>alert('Plaese try again...') </ script >");
            }
            return Json(data, JsonRequestBehavior.AllowGet);


        }
        //function to deduct the money in ewallet of the user
        public ActionResult Deduct()
        {
            var data = new List<object>();
            var amount = Request["amount"];
            string email = Session["email"].ToString();
            string moneyS = Session["bal"].ToString();
            var address = Request["address"];
            var paym = Request["paym"];
            var lname = Request["lname"];
            var fname = Request["fname"];
            double money = Convert.ToDouble(moneyS) - Convert.ToDouble(amount);
            try
            {
                //will update the total money
                if(Convert.ToDouble(amount) <=  money)
                {
                using (var db = new SqlConnection(connDB))
                {
                    db.Open();
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE USERTBL SET MONEY='" + money + "' WHERE EMAIL='" + email + "'";
                        var ctr = cmd.ExecuteNonQuery();
                        if (ctr >= 1)
                        {
                            Response.Write("<script>alert('Processing...') </ script >");

                        }
                        else
                        {
                            Response.Write("<script>alert('Plaese try again...') </ script >");
                        }
                    }
                }

                }
                else
                {
                    Response.Write("<script>alert('Insufficient wallet balance.. ') </ script >");
                }
                //will insert data in payment tbl
                using (var db = new SqlConnection(connDB))
                {
                    db.Open();
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO PAYMENT (FNAME, LNAME, ADDRESS, PAYMETHOD, EMAIL, AMT)"
                            +"VALUES(@NAME,@LASTN,@ADD,@PAY,@MAIL,@AMOUNT)";
                        cmd.Parameters.AddWithValue("@NAME",fname);
                        cmd.Parameters.AddWithValue("@LASTN",lname);
                        cmd.Parameters.AddWithValue("@ADD",address);
                        cmd.Parameters.AddWithValue("@PAY",paym);
                        cmd.Parameters.AddWithValue("@MAIL",email);
                        cmd.Parameters.AddWithValue("@AMOUNT",amount);
                        var ctr = cmd.ExecuteNonQuery();
                        if (ctr >= 1)
                        {
                            Response.Write("<script>alert('Payment Completed...') </ script >");

                        }
                        else
                        {
                            Response.Write("<script>alert('Plaese try again...') </ script >");
                        }
                    }
                }

            }
        
            catch (Exception)
            {
                Response.Write("<script>alert('Plaese try again...') </ script >");
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Ewallet()
        {
            return View();

        }
        public ActionResult UpdateProduct()
        {
            return View();

        }
        //Search the product using the item name 
        public ActionResult SearchItem()
        {
            var data = new List<object>();
            var itmcode = Request["itemcode"];
            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM ITMTBL WHERE ITMNUM='" + itmcode + "'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        data.Add(new
                        {
                            mess = 0,
                            desc = reader["itmname"].ToString(),
                            price = reader["itmprice"].ToString(),
                            onhand = reader["itmonhand"].ToString(),
                        }) ;
                    }
                    else
                    {
                        data.Add(new
                        {
                            mess = 1,
                        });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Update the item 
        public ActionResult UpdateItem()
        {
            var data = new List<object>();

            var itmcode = Request["itemcode"];
            var itmname = Request["itmname"];
            var itmprice = Request["itemprice"];


            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE ITMTBL SET "
                        + " ITMNAME ='" + itmname + "',"
                        + " ITMPRICE ='" + itmprice + "'"
                        + " WHERE ITMNUM='" + itmcode + "'";
                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr > 0)
                    {
                        data.Add(new
                        {
                            mess = 0
                        });
                    }
                    else
                    {
                        data.Add(new
                        {
                            mess = 1
                        });
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveCart()
        {

            var itmcode = Request["itmnum"];
            var cleaned = itmcode.Replace("\n", "").Replace("\r", "");
            var itmnum = Request["itmnum"].Trim();


            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd1 = db.CreateCommand())
                {
                    cmd1.CommandType = CommandType.Text; //DELETE FROM table_name WHERE condition;
                    cmd1.CommandText = "DELETE FROM ORDERTBL WHERE ITMNO ='" + itmnum + "'";
                    cmd1.ExecuteReader();

                }
                db.Close();

            }


            return Json(itmnum, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveItem()
        {

            var itmcode = Request["itmnum"].Trim();
            


            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd1 = db.CreateCommand())
                {
                    cmd1.CommandType = CommandType.Text; //DELETE FROM table_name WHERE condition;
                    cmd1.CommandText = "DELETE FROM ITMTBL WHERE ITMNUM ='" + itmcode + "'";
                    cmd1.ExecuteReader();

                }
                db.Close();

            }


            return Json(itmcode, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageProducts()
        {
            return View();

        } 
        public ActionResult Completed()
        {
            var email = Request["emial"].Trim();



            using (var db = new SqlConnection(connDB))
            {
                db.Open();
                using (var cmd1 = db.CreateCommand())
                {
                    cmd1.CommandType = CommandType.Text; //DELETE FROM table_name WHERE condition;
                    cmd1.CommandText = "DELETE FROM PAYMENT WHERE EMAIL ='" + email + "'";
                    cmd1.ExecuteReader();

                }
                db.Close();

            }


            return Json(email, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("LogIn", "Home");

        }
        public ActionResult Reports()
        {
            return View();

        } 
        public ActionResult Sample()
        {
            return View();

        }


    }
}

