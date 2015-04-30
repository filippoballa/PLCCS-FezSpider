using MvcApplicationTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MvcApplicationTest.Controllers
{
    public class ImgController : ApiController
    {
        public string[] Get(string id = "", string data = "")
        {
            
            return new string[]
                        {
                             id,
                             data
                        };
        }
        public HttpResponseMessage Post(Img i)
        {

            //User = StartRecognition();
            //if(User!= null){
            var response = Request.CreateResponse<String>(System.Net.HttpStatusCode.Created, "Id : " + i.Id);
            //}
            //else{
                //  var response = Request.CreateResponse<String>(System.Net.HttpStatusCode.InternalServerError, "Id : " + i.Id);
            //}
            return response;
        }

        /*
             * DATA WITH SHA1
             * 
             * byte[] data = GetBytes(s);
            
             * byte[] result;

             * SHA1 sha = new SHA1CryptoServiceProvider();
             * // This is one implementation of the abstract class SHA1.
             * result = sha.ComputeHash(data);
             * return new string[]
                        {
                             s,
                             GetString(result)
                        };
             */
        /*static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
         * */

    }
}
