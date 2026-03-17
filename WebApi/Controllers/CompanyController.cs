using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Company>>> GetAll()
    {
        var companies = await _companyService.GetAllAsync();
        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Company>> GetById(int id)
    {
        var company = await _companyService.GetByIdAsync(id);
        if (company == null) return NotFound($"Компания с id {id} не найдена");
        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(Company company)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var id = await _companyService.CreateAsync(company);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Company company)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _companyService.UpdateAsync(id, company);
        if (!result) return NotFound($"Компания с id {id} не найдена");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _companyService.DeleteAsync(id);
        if (!result) return NotFound($"Компания с id {id} не найдена");
        return NoContent();
    }
}