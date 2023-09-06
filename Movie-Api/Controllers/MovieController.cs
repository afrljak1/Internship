using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Api.Models;

namespace Movie_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MovieController : ControllerBase
    {
        private readonly APIDbContext _context;

        public MovieController(APIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie()
        {
            return await _context.Movies.ToListAsync();
        }

     
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovieById(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchMoviessByName(string name)
        {
            var movies = await _context.Movies
                .Where(s => s.Title.Contains(name))
                .ToListAsync();

            if (movies == null)
            {
                return NotFound();
            }

            return Ok(movies);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Movie), 201)]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.MovieId }, movie);
        }


        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteMovie(string name)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Title == name);

            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }



       /* [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteMovieID(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(id => id.MovieId == id);

            if(movie==null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Movie), 204)]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.MovieId)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); //204 No Content
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }

    }
}
