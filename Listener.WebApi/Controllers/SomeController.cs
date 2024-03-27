using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQLib.Interfaces;
using RabbitMQLib.Models;
using System.Text;

namespace Listener.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SomeController : ControllerBase
    {
        private readonly IRPCClient _client;

        public SomeController(IRPCClient client)
        {
            _client = client;
        }

        [HttpPost]
        public async Task<ActionResult> SomeTask(CancellationToken ct)
        {
            try
            {
                var content = await SerializeContext(Request, false);
                var requestString = Encoding.UTF8.GetString(content);
                var message = JsonConvert.DeserializeObject<Message>(requestString);

                // Delay to simulate processing
                await Task.Delay(5000, ct);

                var result = _client.SendRequestWithReply(message, "queue_rpc");

                if (result.HasError)
                {
                    return StatusCode(500, result.ErrorDescription);
                }

                var serializedResult = JsonConvert.SerializeObject(result);
                return Ok(serializedResult);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return StatusCode(500, "Task Canceled due to token cancellation.");
            }
            catch (OperationCanceledException ctex)
            {
                return StatusCode(500, $"Task Canceled due to timeout. Exception Message: {ctex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error. Exception Message: {ex.Message}");
            }
        }



        public const Int32 BUFFER_SIZE = 256 * 1024;

        [NonAction]
        public static async Task<byte[]> SerializeContext(HttpRequest request, bool isTokenRequest)
        {
            using (var mem = new MemoryStream())
            {
                Byte[] lBuffer = new Byte[BUFFER_SIZE];
                Int32 lBytesRead;
                do
                {
                    lBytesRead = await request.Body.ReadAsync(lBuffer, 0, BUFFER_SIZE);
                    if (lBytesRead != 0) await mem.WriteAsync(lBuffer, 0, lBytesRead);
                }
                while (lBytesRead > 0);
                return mem.ToArray();
            }
        }
    }
}
