using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize] // Authenticated users can pay
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> ProcessPayment([FromBody] ProcessPaymentDto dto)
        {
            var result = await _paymentService.ProcessPaymentAsync(dto);
            if (!result.IsSuccess)
            {
                if (result.Error.Contains("not found", System.StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error });
                }
                return BadRequest(new ProblemDetails { Title = "Payment Failed", Detail = result.Error });
            }
            return Ok(result.Value);
            return Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return Ok(result.Value);
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> GetById(long id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Error });
            }
            return Ok(result.Value);
        }

        [HttpGet("booking/{bookingId:long}")]
        [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByBookingId(long bookingId)
        {
            var result = await _paymentService.GetPaymentsByBookingIdAsync(bookingId);
            return Ok(result.Value);
        }
    }
}
