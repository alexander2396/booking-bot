using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;

namespace BookingApiBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private static readonly JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

        [HttpPost]
        public JsonResult DialogAction()
        {
            WebhookRequest request;

            using (var reader = new StreamReader(Request.Body))
            {
                request = jsonParser.Parse<WebhookRequest>(reader.ReadToEnd());
            }

            double totalAmount = 0;
            double totalNights = 0;
            double totalPersons = 0;

            if (request.QueryResult.Action == "book")
            {
                var requestParameters = request.QueryResult.Parameters;
                totalPersons = requestParameters.Fields["totalPersons"].NumberValue;
                totalNights = requestParameters.Fields["totalNights"].NumberValue;
                totalAmount = totalNights * 100;
            }

            WebhookResponse response = new WebhookResponse
            {
                FulfillmentText = $"Thank you for choosing our hotel, your total amount for the {totalNights} nights for {totalPersons} persons will be {totalAmount} USD."
            };

            string responseJson = response.ToString();
            return Json(responseJson);
        }
    }
}