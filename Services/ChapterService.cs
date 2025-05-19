using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Repository.IRepository;
using RESTfulBookWebsite.Response;
using RESTfulBookWebsite.Services.IServices;
using static System.Reflection.Metadata.BlobBuilder;

namespace RESTfulBookWebsite.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IBooksRepository _booksRepo;
        private readonly IChaptersRepository _chaptersRepo;
        private readonly IReviewsRepository _reviewsRepo;
        private readonly IMapper _mapper;

        public ChapterService(IBooksRepository booksRepo, IChaptersRepository chaptersRepo, IReviewsRepository reviewsRepo, IMapper mapper)
        {
            _booksRepo = booksRepo;
            _chaptersRepo = chaptersRepo;
            _reviewsRepo = reviewsRepo;
            _mapper = mapper;
        }
        public async Task<ServiceResponseObject<ChapterDTO>> CreateChapter(ChapterDTO chapterDTO)
        {
            var chapter = _mapper.Map<Chapter>(chapterDTO);

            chapter.CreatedDate = DateTime.UtcNow;
            chapter.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _chaptersRepo.CreateAsync(chapter);
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorMessage = $"Database update failed: {ex.Message}",
                    ErrorCode = 409
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error occurred: {ex.Message}",
                    ErrorCode = 500
                };
            }

            var savedChapter = _mapper.Map<ChapterDTO>(chapter);

            return new ServiceResponseObject<ChapterDTO>
            {
                Success = true,
                Data = savedChapter
            };
        }

        public async Task<ServiceResponseObject<ChapterDTO>> DeleteChapter(int id)
        {
            var chapter = await _chaptersRepo.GetAsync(a => a.Id == id);

            if (chapter == null)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {id} not found."
                };
            }

            try
            {
                await _chaptersRepo.RemoveAsync(chapter);
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"Failed to delete author: {ex.Message}"
                };
            }

            return new ServiceResponseObject<ChapterDTO>
            {
                Success = true,
            };
        }

        public async Task<IEnumerable<ChapterDTO>> GetAllChaptersAsync()
        {
            var chapters = await _chaptersRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<ChapterDTO>>(chapters);
        }

        public async Task<ChapterDTO> GetChapter(int id)
        {
            var chapter = await _chaptersRepo.GetAsync(u => u.Id == id);

            if (chapter == null) { return null; }

            var chapterDTO = _mapper.Map<ChapterDTO>(chapter);

            return chapterDTO;
        }

        public async Task<ChapterDTO> GetChapter(string title)
        {
            var chapter = await _chaptersRepo.GetAsync(u => u.Title.ToLower() == title.ToLower());

            if (chapter == null) { return null; }

            var chapterDTO = _mapper.Map<ChapterDTO>(chapter);

            return chapterDTO;
        }

        public async Task<ServiceResponseObject<IEnumerable<ChapterDTO>>> GetChaptersByBook(string bookName)
        {
            var book = await _booksRepo.GetAsync(u => u.Name.ToLower() == bookName.ToLower());

            if (book == null)
            {
                return new ServiceResponseObject<IEnumerable<ChapterDTO>>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"No books found with name '{bookName}'."
                };
            }

            var chapters = await _chaptersRepo.GetAllAsync(u => u.BookID == book.Id);

            var chapterDTOs = _mapper.Map<IEnumerable<ChapterDTO>>(chapters);


            return new ServiceResponseObject<IEnumerable<ChapterDTO>>
            {
                Success = true,
                Data = chapterDTOs
            };

        }

        public async Task<ServiceResponseObject<ChapterDTO>> UpdateChapterAsync(ChapterDTO chapterDTO)
        {
            var existingChapter = await _chaptersRepo.GetAsync(a => a.Id == chapterDTO.Id);

            if (existingChapter == null)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {chapterDTO.Id} not found."
                };
            }

            existingChapter.Title = chapterDTO.Title;
            existingChapter.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _chaptersRepo.UpdateAsync(existingChapter);
                var updatedDTO = _mapper.Map<ChapterDTO>(existingChapter);

                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = true,
                    Data = updatedDTO
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponseObject<ChapterDTO>> UpdateChapterPartialAsync(int id, JsonPatchDocument<ChapterDTO> patchDTO)
        {
            var existingChapter = await _chaptersRepo.GetAsync(a => a.Id == id);

            if (existingChapter == null)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {id} not found."
                };
            }

            var ChapterDTO = _mapper.Map<ChapterDTO>(existingChapter);

            try
            {
                patchDTO.ApplyTo(ChapterDTO);

                var updatedChapter = _mapper.Map<Chapter>(ChapterDTO);

                updatedChapter.UpdatedDate = DateTime.UtcNow;

                await _chaptersRepo.UpdateAsync(updatedChapter);

                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = true,
                    Data = _mapper.Map<ChapterDTO>(updatedChapter)
                };
            }
            catch (JsonPatchException ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 400,
                    ErrorMessage = $"Patch operation failed: {ex.Message}"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed due to conflict: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<ChapterDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        Task<IEnumerable<ChapterDTO>> IChapterService.GetChaptersByBook(string bookName)
        {
            throw new NotImplementedException();
        }
    }
}
