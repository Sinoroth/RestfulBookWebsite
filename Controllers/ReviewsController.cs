using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Models.Dto;
using UdemiCourse.Data;

namespace RESTfulBookWebsite.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ILogger<ReviewsController> logger;

        private readonly ApplicationDBContext _db;

        public ReviewsController(ApplicationDBContext db, ILogger<ReviewsController> _logger)
        {
            _db = db;
            logger = _logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Review>> GetReviews()
        {
            logger.LogInformation("Getting all Reviews");
            return Ok(_db.Reviews.ToList());
        }

        [HttpGet]
        [Route("GetByAuthorName")]
        public ActionResult<IEnumerable<Review>> GetReviewsByAuthor(string authorName)
        {
            var reviews = _db.GetReviewsByAuthor(authorName);

            if (reviews.Count == 0)
                return NotFound();

            return reviews;
        }

        [HttpGet("{id:int}", Name = "GetReview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ReviewDTO> GetReview(int id)
        {
            if (id == 0)
            {
                logger.LogError("Get Review error with ID of " + id);
                return BadRequest("Bad Id");
            }
            var review = _db.Reviews.FirstOrDefault(x => x.Id == id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReviewDTO> CreateReview([FromBody] ReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_db.Reviews.FirstOrDefault(u => u.Id == reviewDTO.Id) != null)
            {
                ModelState.AddModelError("Custom Error", "Review already exists!");
                return BadRequest(ModelState);
            }
            if (reviewDTO == null)
            {
                return BadRequest(reviewDTO);
            }
            if (reviewDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Review model = new()
            {
                Id = reviewDTO.Id,
                BookId = reviewDTO.BookId,
                UserId = reviewDTO.UserId,
                Comment = reviewDTO.Comment,
                Rating = reviewDTO.Rating
            };
            _db.Reviews.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetReview", new { id = reviewDTO.Id }, reviewDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteReview")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteReview(int id)
        {
            if (id == 0) return BadRequest();

            var reviews = _db.Reviews.FirstOrDefault(u => u.Id == id);

            if (reviews == null) return NotFound();

            _db.Reviews.Remove(reviews);
            _db.SaveChanges();
            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateReview")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateReview(int id, [FromBody] ReviewDTO reviewDTO)
        {
            if (reviewDTO == null || reviewDTO.Id != id) { return BadRequest(); }

            Review model = new()
            {
                Id = reviewDTO.Id,
                BookId = reviewDTO.BookId,
                UserId = reviewDTO.UserId,
                Comment = reviewDTO.Comment,
                Rating = reviewDTO.Rating
            };

            _db.Reviews.Update(model);
            _db.SaveChanges();

            return NoContent();
        }
        [HttpPatch("{id:int}", Name = "UpdatePartialReview")]
        public IActionResult UpdatePartialReview(int id, JsonPatchDocument<ReviewDTO> PatchDTO)
        {
            if (PatchDTO == null || id == 0) { return NotFound(); }

            var review = _db.Reviews.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (review == null) { return BadRequest(); }

            ReviewDTO reviewDTO = new()
            {
                Id = review.Id,
                BookId = review.BookId,
                UserId = review.UserId,
                Comment = review.Comment,
                Rating = review.Rating
            };

            PatchDTO.ApplyTo(reviewDTO, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Review model = new()
            {
                Id = reviewDTO.Id,
                BookId = reviewDTO.BookId,
                UserId = reviewDTO.UserId,
                Comment = reviewDTO.Comment,
                Rating = reviewDTO.Rating
            };

            _db.Update(model);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
