using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zad1.DTOs;
using zad1.Exceptions;
using zad1.Models;
using zad1.Services;

namespace zad1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        private readonly IDbService _dbService;

        public HospitalController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription(NewPrescriptionDTO prescription)
        {
            try
            {
                await _dbService.AddPrescription(prescription);
                return Created();
            }
            catch (BadRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
