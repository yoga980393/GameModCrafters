using GameModCrafters.Data;
using GameModCrafters.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameModCrafters.Services
{
    public class ModService
    {
        private readonly ApplicationDbContext _context;

        public ModService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person_PagedMods> GetPublishedMods(string id, int currentPage, int pageSize)
        {
            var totalCount = await _context.Mods.Where(m => m.AuthorId == id && m.IsDone).CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var mods = await _context.Mods
                .Where(m => m.AuthorId == id)
                .Where(m => m.IsDone)
                .OrderByDescending(m => m.CreateTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Author)
                .Include(m => m.Game)
                .Include(m => m.ModLikes)
                .Include(m => m.Favorite)
                .Include(m => m.Downloaded)
                .Include(m => m.ModTags)
                    .ThenInclude(mt => mt.Tag)
                .Select(m => new ModViewModel
                {
                    ModId = m.ModId,
                    Thumbnail = m.Thumbnail,
                    ModName = m.ModName,
                    Price = m.Price,
                    GameName = m.Game.GameName,
                    AuthorName = m.Author.Username,
                    CreateTime = m.CreateTime,
                    UpdateTime = m.UpdateTime,
                    Description = m.Description,
                    Capacity = 0,
                    LikeCount = m.ModLikes.Count,
                    FavoriteCount = m.Favorite.Count,
                    DownloadCount = m.Downloaded.Count,
                    TagNames = m.ModTags.Select(mt => mt.Tag.TagName).ToList()
                })
                .ToListAsync();

            var vm = new Person_PagedMods()
            {
                Mods = mods,
                TotalPages = totalPages
            };

            return vm;
        }

        public async Task<Person_PagedMods> GetFavoritedMods(string id, int currentPage, int pageSize)
        {
            var totalCount = await _context.Mods.Where(m => m.Favorite.Any(f => f.UserId == id) && m.IsDone).CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var mods = await _context.Mods
                .Where(m => m.Favorite.Any(f => f.UserId == id))
                .Where(m => m.IsDone)
                .OrderByDescending(c => c.CreateTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Author)
                .Include(m => m.Game)
                .Include(m => m.ModLikes)
                .Include(m => m.Favorite)
                .Include(m => m.Downloaded)
                .Include(m => m.ModTags)
                    .ThenInclude(mt => mt.Tag)
                .Select(m => new ModViewModel
                {
                    ModId = m.ModId,
                    Thumbnail = m.Thumbnail,
                    ModName = m.ModName,
                    Price = m.Price,
                    GameName = m.Game.GameName,
                    AuthorName = m.Author.Username,
                    CreateTime = m.CreateTime,
                    UpdateTime = m.UpdateTime,
                    Description = m.Description,
                    Capacity = 0,
                    LikeCount = m.ModLikes.Count,
                    FavoriteCount = m.Favorite.Count,
                    DownloadCount = m.Downloaded.Count,
                    TagNames = m.ModTags.Select(mt => mt.Tag.TagName).ToList()
                })
                .ToListAsync();

            var vm = new Person_PagedMods()
            {
                Mods = mods,
                TotalPages = totalPages
            };

            return vm;
        }

        public async Task<Person_PagedMods> GetDownloadedMods(string id, int currentPage, int pageSize)
        {
            var totalCount = await _context.Mods.Where(m => m.Downloaded.Any(d => d.UserId == id) && m.IsDone).CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var mods = await _context.Mods
                .Where(m => m.Downloaded.Any(d => d.UserId == id))
                .Where(m => m.IsDone)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Author)
                .Include(m => m.Game)
                .Include(m => m.ModLikes)
                .Include(m => m.Favorite)
                .Include(m => m.Downloaded)
                .Include(m => m.ModTags)
                    .ThenInclude(mt => mt.Tag)
                .Select(m => new ModViewModel
                {
                    ModId = m.ModId,
                    Thumbnail = m.Thumbnail,
                    ModName = m.ModName,
                    Price = m.Price,
                    GameName = m.Game.GameName,
                    AuthorName = m.Author.Username,
                    CreateTime = m.CreateTime,
                    UpdateTime = m.UpdateTime,
                    Description = m.Description,
                    Capacity = 0,
                    LikeCount = m.ModLikes.Count,
                    FavoriteCount = m.Favorite.Count,
                    DownloadCount = m.Downloaded.Count,
                    TagNames = m.ModTags.Select(mt => mt.Tag.TagName).ToList()
                })
                .ToListAsync();

            var vm = new Person_PagedMods()
            {
                Mods = mods,
                TotalPages = totalPages
            };

            return vm;
        }

    }
}
