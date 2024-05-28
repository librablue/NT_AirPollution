using Newtonsoft.Json;
using NT_AirPollution.Admin.ActionFilter;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace NT_AirPollution.Admin.Controllers
{
    public class AdminController : ApiController
    {
        private readonly AdminService _adminService = new AdminService();
        public AjaxResult Login(AdminUser user)
        {
            try
            {
                if (HttpContext.Current.Session["Captcha"] == null || user.Captcha.ToUpper() != HttpContext.Current.Session["Captcha"].ToString().ToUpper())
                    throw new Exception("驗證碼錯誤");

                HttpContext.Current.Session.Remove("Captcha");
                HttpContext.Current.Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);

                var result = _adminService.Login(user);
                if (result == null)
                    throw new Exception("登入失敗，帳號或密碼錯誤");

                // 清空密碼資訊
                result.Password = null;
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    result.Account,
                    DateTime.Now,
                    DateTime.Now.AddHours(8),
                    true,
                    JsonConvert.SerializeObject(result),
                    FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                return new AjaxResult { Status = true, Message = result };
            }
            catch (Exception ex)
            {
                return new AjaxResult { Status = false, Message = ex.Message };
            }
        }

        [AuthorizeUser]
        public AdminUser GetCurrentUser()
        {
            return BaseService.CurrentAdmin;
        }

        [HttpGet]
        public void Logout()
        {
            HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-1);
        }

        [HttpGet]
        public HttpResponseMessage Captcha()
        {
            // 取得驗證碼字串
            string verifyCode = GenerateCheckCode(4);
            HttpContext.Current.Session["Captcha"] = verifyCode;

            using (var ms = new MemoryStream())
            {
                using (var bitmap = new Bitmap(140, 45))        //尺寸。
                {
                    using (var graphic = Graphics.FromImage((System.Drawing.Image)bitmap))
                    {
                        //颜色列表
                        Color[] colors = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkBlue };
                        Random rnd = new Random();
                        graphic.Clear(Color.White);         // 清空圖片背景色。
                        for (int i = 0; i < 10; i++)        // 畫圖片的背景噪音線。
                        {
                            int x1 = rnd.Next(bitmap.Width);
                            int x2 = rnd.Next(bitmap.Width);
                            int y1 = rnd.Next(bitmap.Height);
                            int y2 = rnd.Next(bitmap.Height);
                            Color color = colors[rnd.Next(colors.Length)];
                            graphic.DrawLine(new Pen(color), x1, y1, x2, y2);
                        }
                        Font font = new System.Drawing.Font("Baskerville Old Face", 30, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
                        for (int i = 0; i < verifyCode.Length; i++)
                        {
                            Color color = colors[rnd.Next(colors.Length)];
                            // 畫出字
                            graphic.DrawString(verifyCode[i].ToString(), font, new SolidBrush(color), Convert.ToSingle(i) * 30, Convert.ToSingle(2));
                        }
                        for (int i = 0; i < 100; i++)
                        {
                            // 畫圖片的前景噪音點
                            int x = rnd.Next(bitmap.Width);
                            int y = rnd.Next(bitmap.Height);
                            bitmap.SetPixel(x, y, Color.FromArgb(rnd.Next()));
                        }
                        // 畫圖片的邊框線
                        graphic.DrawRectangle(new Pen(Color.Silver), 0, 0, bitmap.Width - 1, bitmap.Height - 1);
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(ms.ToArray());
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                return response;
            }
        }

        /// <summary>
        /// 回傳驗證碼字串
        /// </summary>
        /// <returns></returns>
        private string GenerateCheckCode(int length)
        {
            string[] codes = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string verifyCode = string.Empty;
            Random rnd = new Random();
            // 四個驗證碼
            for (int i = 0; i < length; i++)
            {
                verifyCode += codes[rnd.Next(10)];
            }
            return verifyCode;
        }

        [AuthorizeUser]
        [HttpPost]
        public AjaxResult UpdatePassword(AdminUser user)
        {
            try
            {
                var currentUser = BaseService.CurrentAdmin;
                _adminService.UpdatePassword(currentUser.Account, user.Password);
                return new AjaxResult { Status = true };
            }
            catch (Exception ex)
            {
                return new AjaxResult { Status = false, Message = ex.Message };
            }
        }

        [AuthorizeUser]
        [HttpPost]
        public List<AdminUser> GetUsers(AdminUserFilterView filter)
        {
            var result = _adminService.GetUsers(filter);
            return result;
        }

        [AuthorizeUser]
        public AjaxResult AddUser(AdminUser user)
        {
            try
            {
                _adminService.AddUser(user);
                return new AjaxResult { Status = true };
            }
            catch (Exception ex)
            {
                return new AjaxResult { Status = false, Message = ex.Message };
            }
        }

        [AuthorizeUser]
        public AjaxResult UpdateUser(AdminUser user)
        {
            try
            {
                _adminService.UpdateUser(user);
                return new AjaxResult { Status = true };
            }
            catch (Exception ex)
            {
                return new AjaxResult { Status = false, Message = ex.Message };
            }
        }
    }
}
