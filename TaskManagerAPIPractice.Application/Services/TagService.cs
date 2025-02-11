using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagerAPIPractice.DataAccess.abstruction;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using TaskManagerAPIPractice.DataAccess.Repositories;

namespace TaskManagerAPIPractice.Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<TagEntity>> GetAllAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<TagEntity?> GetByIdAsync(Guid id)
        {
            return await _tagRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TagEntity tag)
        {
            await _tagRepository.AddAsync(tag);
        }

        public async Task UpdateAsync(TagEntity tag)
        {
            await _tagRepository.UpdateAsync(tag);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _tagRepository.DeleteAsync(id);
        }
    }
}
