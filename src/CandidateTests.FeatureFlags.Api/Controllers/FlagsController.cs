using CandidateTests.FeatureFlags.Api.Data;
using CandidateTests.FeatureFlags.Api.Evaluation;
using Microsoft.AspNetCore.Mvc;

namespace CandidateTests.FeatureFlags.Api.Controllers;

[ApiController]
[Route("flags")]
public class FlagsController : ControllerBase
{
    private readonly IFlagDao _flagDao;
    private readonly FlagEvaluator _evaluator;

    public FlagsController(IFlagDao flagDao, FlagEvaluator evaluator)
    {
        _flagDao = flagDao;
        _evaluator = evaluator;
    }

    [HttpGet]
    public async Task<IActionResult> GetFlags()
    {
        var flags = await _flagDao.GetAllAsync();
        return Ok(flags);
    }

    [HttpGet]
    [Route("{key}")]
    public async Task<IActionResult> GetFlag(string key)
    {
        var flag = await _flagDao.GetByKeyAsync(key);
        if (flag is null)
        {
            return NotFound();
        }

        return Ok(flag);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlag([FromBody] Flag input)
    {
        input.CreatedAt = DateTime.UtcNow;
        input.UpdatedAt = input.CreatedAt;
        var created = await _flagDao.InsertAsync(input);

        return Created($"/flags/{created.Key}", created);
    }

    [HttpPut]
    [Route("{key}")]
    public async Task<IActionResult> UpdateFlag(string key, [FromBody] Flag input)
    {
        input.UpdatedAt = DateTime.UtcNow;
        var updated = await _flagDao.UpdateAsync(key, input);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete]
    [Route("{key}")]
    public async Task<IActionResult> DeleteFlag(string key)
    {
        var deleted = await _flagDao.DeleteAsync(key);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet]
    [Route("/evaluate")]
    public async Task<IActionResult> Evaluate([FromQuery] string userId, [FromQuery] string flagKey)
    {
        var flag = await _flagDao.GetByKeyAsync(flagKey);
        if (flag is null)
        {
            return NotFound();
        }

        var result = _evaluator.Evaluate(flag, userId);
        return Ok(new { enabled = result });
    }
}
