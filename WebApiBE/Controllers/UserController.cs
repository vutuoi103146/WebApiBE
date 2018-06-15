

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiBE.BL;
using WebApiBE.Models;

namespace WebApiBE.Controllers
{
    public class UserController : ApiController
    {
        private ErpEntities db = new ErpEntities();

        // GET api/User
        public IQueryable<T3UsersLogin> GetT3UsersLogin()
        {
            IQueryable<T3UsersLogin> iRet = db.T3UsersLogin;
            foreach (T3UsersLogin item in iRet)
            {
                item.Password = L3CryptorEngine.Decrypt(item.Password);
            }
            return iRet;
        }

        // GET api/User/5
        [ResponseType(typeof(T3UsersLogin))]
        public IHttpActionResult GetT3UsersLogin(string id)
        {
            T3UsersLogin t3userslogin = db.T3UsersLogin.Find(id);
            if (t3userslogin == null)
            {
                return NotFound();
            }
            t3userslogin.Password = L3CryptorEngine.Decrypt(t3userslogin.Password);
            return Ok(t3userslogin);
        }

      

        [HttpGet]
        [Route("api/User/UserActive")]
        public bool UserActive(string username, string keyString)
        {
            try
            {
                T3UsersLogin t3user = db.T3UsersLogin.First(e => e.UserName == username && e.KeyStringValidEmail == keyString);
                t3user.IsApply = 1;
                db.Entry(t3user).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
           
        }


        // PUT api/User/5
        public IHttpActionResult PutT3UsersLogin(string id, T3UsersLogin t3userslogin)
        {
            t3userslogin.ModifyDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != t3userslogin.UserName)
            {
                return BadRequest();
            }
            t3userslogin.Password = L3CryptorEngine.Encrypt(t3userslogin.Password);
            db.Entry(t3userslogin).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!T3UsersLoginExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // POST api/User
        [ResponseType(typeof(T3UsersLogin))]
        public IHttpActionResult PostT3UsersLogin(T3UsersLogin t3userslogin)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           // string url = t3usersloginExt.Url;
            t3userslogin.Password = L3CryptorEngine.Encrypt(t3userslogin.Password);
            var now = DateTime.Now;
            t3userslogin.ModifyDate = now;
            t3userslogin.CreateDate = now;
            t3userslogin.LastLogin = null;
            t3userslogin.KeyStringValidEmail = L3Common.RandomString(25);
            db.T3UsersLogin.Add(t3userslogin);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (T3UsersLoginExists(t3userslogin.UserName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            SendMail(t3userslogin);
            return CreatedAtRoute("DefaultApi", new { id = t3userslogin.UserName }, t3userslogin);
        }

        private void SendMail(T3UsersLogin user)
        {
            string sSubject= "Mail kích hoạt tài khoản";
            string sMailBody = "Chào bạn: " + user.FirstName + " " + user.LastName;
            sMailBody += " <br /> Bạn vừa đăng ký tài khoản vào trang web của chúng tôi. Bạn vui lòng chọn đường Link bên dưới để xác nhận tài khoản. Nếu không vui lòng bỏ qua Email này. <br /> ";
            string queryParam = "username=" + user.UserName + "&keyString=" + user.KeyStringValidEmail;
            sMailBody += @"<a href='" + user.Url + "?" + queryParam  + "'>XÁC NHẬN KÍCH HOẠT TÀI KHOẢN</a>";
            if (CheckSendMail(user.Email)) L3Email.SendMail(user.Email, sSubject, sMailBody, "");
        }
        private bool CheckSendMail(string email)
        {
            if (email == null || email.Trim() == "") return false;
            return true;
        }
        // DELETE api/User/5
        [ResponseType(typeof(T3UsersLogin))]
        public IHttpActionResult DeleteT3UsersLogin(string id)
        {
            T3UsersLogin t3userslogin = db.T3UsersLogin.Find(id);
            if (t3userslogin == null)
            {
                return NotFound();
            }

            db.T3UsersLogin.Remove(t3userslogin);
            db.SaveChanges();

            return Ok(t3userslogin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool T3UsersLoginExists(string id)
        {
            return db.T3UsersLogin.Count(e => e.UserName == id) > 0;
        }
    }
}