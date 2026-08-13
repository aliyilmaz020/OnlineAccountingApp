using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OnlineAccountingApp.WebApi.Controllers
{
    public class TestController(IMediator mediator) : BaseApiController(mediator)
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Success");
        }
    }
}
