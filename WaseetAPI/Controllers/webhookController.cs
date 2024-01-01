using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaseetAPI.Application;
using WaseetAPI.Application.Salla;
using WaseetAPI.Database;
//using WaseetAPI.Domain.Models.Salla;

namespace WaseetAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class webhookController : Controller
    {
        private readonly MainDbContext _ctx;
        public webhookController(MainDbContext ctx)
        {
            _ctx = ctx;
        }
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            // Process data received from Enterprise Communications API
            try
            {
                // Grab the body and parse to a JSON object
                string rawBody = GetDocumentContents(Request);
                if (string.IsNullOrEmpty(rawBody))
                {
                    // No body, bad request.
                    return BadRequest("Bad request - No JSON body found!");
                }

                // We have a request body so lets look at what we have

                // First lets ensure it hasn't been tampered with and it came from Enterprise Communications API
                // We do this by checking the HMAC from the X-Enterprise Communications API-Signature header
                //==string hmac = Request.Headers["x-comapi-signature"];

                //==if (String.IsNullOrEmpty(hmac))
                //=={
                // No HMAC, invalid request.
                //==RollingLogger.LogMessage("Invalid request: No HMAC value found!");
                //==return Unauthorized();
                //==}
                //==else
                //=={
                // Validate the HMAC, ensure you has exposed the rawBody, see app.js for how to do this
                //==var hash = CreateHMAC(rawBody, ">>>YOUR SECRET<<<");

                //==if (hmac != hash)
                //=={
                // The request is not from Enterprise Communications API or has been tampered with
                //==RollingLogger.LogMessage("Invalid request: HMAC hash check failed!");
                //==return Unauthorized();
                //==}
                //==}

                // Parse the recieved JSON to extract data
                //==dynamic eventObj = JsonConvert.DeserializeObject(rawBody);
                bool is_save_receive_data = await new ReceiveProcedures(_ctx).save_receive_data(rawBody);
                if (!is_save_receive_data)
                    await new ReceiveProcedures(_ctx).savelog(rawBody, 1);

                // Store the received event for later processing, remember you only have 10 secs to process, in this simple example we output to the console
                //==RollingLogger.LogMessage("");
                //==RollingLogger.LogMessage(String.Format("Received a {0} event id: {1}", (string)eventObj.name, (string)eventObj.eventId));
                //==RollingLogger.LogMessage(FormatJson(rawBody));

                // You could use queuing tech such as RabbitMQ, MSMQ or possibly a distributed cache such as Redis

                // All good return a 200
                return Ok("Data accepted");
            }
            catch (Exception err)
            {
                // An error occurred
                var msg = "An error occurred receiving data, the error was: " + err.ToString();
                await new ReceiveProcedures(_ctx).savelog(msg, 2);
                //==RollingLogger.LogMessage(msg);
                throw;
            }
        }
        //[HttpPost("create")]
        //public async Task<IActionResult> create(SallaReceive new_receive_data)
        //{
        //    string new_receive_data_string = JsonConvert.SerializeObject(new_receive_data);
        //    await new ReceiveProcedures(_ctx).save_receive_data(new_receive_data_string);

        //    return Ok("Data accepted");
        //    //else
        //    //return await new InvoiceProcedures(connectionStr).CreateInvoice(user_id, null);
        //}
        /// <summary>
        /// Creates a HMAC-SHA1 hash
        /// </summary>
        /// <param name="data">The data to be hashed</param>
        /// <param name="key">The secret to use as a crypto key</param>
        /// <returns>HMAC-SHA1 hash for the data</returns>
        /*private string CreateHMAC(string data, string key)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);

            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(data);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);

            return ByteToString(hashmessage);
        }

        /// <summary>
        /// Converts a byte array to hex string
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary.ToLower());
        }
        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
         */

        /// <summary>
        /// Retrieves the body of a HTTP request as a string
        /// </summary>
        /// <param name="Request">The HTTP Request</param>
        /// <returns>The body data as a string</returns>
        private string GetDocumentContents(HttpRequest Request)
        {
            string documentContents;
            using (Stream receiveStream = Request.Body)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }
    }
}
