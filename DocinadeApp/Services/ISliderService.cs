using DocinadeApp.Models;

namespace DocinadeApp.Services
{
    public interface ISliderService
    {
        Task<IEnumerable<SliderItem>> GetAllAsync();
        Task<IEnumerable<SliderItem>> GetActiveAsync();
        Task<SliderItem?> GetByIdAsync(int id);
        Task<SliderItem> CreateAsync(SliderItem sliderItem, IFormFile? imageFile, string userId);
        Task<SliderItem> UpdateAsync(SliderItem sliderItem, IFormFile? imageFile, string userId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleActiveAsync(int id);
        Task<string> SaveImageAsync(IFormFile imageFile);
        Task DeleteImageAsync(string imageUrl);
    }
}
