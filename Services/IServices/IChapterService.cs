using Microsoft.AspNetCore.JsonPatch;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Response;

namespace RESTfulBookWebsite.Services.IServices
{
    public interface IChapterService
    {
        Task<IEnumerable<ChapterDTO>> GetAllChaptersAsync();
        Task<ChapterDTO> GetChapter(int id);
        Task<ChapterDTO> GetChapter(string name);
        Task<ServiceResponseObject<ChapterDTO>> CreateChapter(ChapterDTO chapterDTO);
        Task<ServiceResponseObject<ChapterDTO>> DeleteChapter(int id);
        Task<ServiceResponseObject<ChapterDTO>> UpdateChapterAsync(ChapterDTO chapterDTO);
        Task<ServiceResponseObject<ChapterDTO>> UpdateChapterPartialAsync(int id, JsonPatchDocument<ChapterDTO> patchDTO);
        Task<IEnumerable<ChapterDTO>> GetChaptersByBook(string bookName);
    }
}
