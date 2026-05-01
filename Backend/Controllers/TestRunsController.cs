using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Domain;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestRunsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestRunsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/TestRuns
        [HttpPost]
        public async Task<IActionResult> CreateTestRun()
        {
            var testRun = new TestRun
            {
                CreatedDate = DateTime.UtcNow,
                Status = TestRunStatus.Pending
            };

            _context.TestRuns.Add(testRun);
            await _context.SaveChangesAsync();

            return Ok(testRun);
        }

        // GET: api/TestRuns
        [HttpGet]
        public async Task<IActionResult> GetTestRuns()
        {
            var runs = await _context.TestRuns.ToListAsync();
            return Ok(runs);
        }
        public void BadMethod()
        {
            int n = 100;
            int i = 0;
            string s = "This is a string";
            if (n == 100)
            {
                Console.WriteLine("Value is 100");
            }
        }
    }
}