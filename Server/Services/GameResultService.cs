using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using cognify.Server.Data;  // Update namespace based on your project structure
using cognify.Shared;
namespace cognify.Server.Services
{
public class GameResultService
{
    private readonly AppDbContext _context;

    public GameResultService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddResultAsync(GameResult result)
    {
        Console.WriteLine("AddResultAsync called with: " + result.Score);
        _context.GameResults.Add(result);
        await _context.SaveChangesAsync();
        Console.WriteLine("Score saved to database.");
    }

    public async Task<List<GameResult>> GetResultsAsync()
    {
        return await _context.GameResults.ToListAsync();
    }
    public async Task<GameResult> GetResultByIdAsync(int id)
    {
        return await _context.GameResults.FindAsync(id);
    }

    public async Task UpdateResultAsync(GameResult result)
    {
        _context.GameResults.Update(result);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteResultAsync(int id)
    {
        var result = await _context.GameResults.FindAsync(id);
        if (result != null)
        {
            _context.GameResults.Remove(result);
            await _context.SaveChangesAsync();
        }
    }
}
}
