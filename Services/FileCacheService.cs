using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace TianyiNetwork.Web.AppsApi.Services
{
    public class FileCacheService<T>(string cachePath) : ISingletonService<FileCacheService<T>> where T : ControllerBase
    {
        public static FileCacheService<T> Create(WebApplicationBuilder builder)
        {
            var className = typeof(T).Name;
            if (!className.EndsWith("Controller")) throw new ArgumentException("The controller class name should end with 'Controller'.");
            var cacheRoot = Path.Combine(builder.Environment.ContentRootPath, "FileCache");
            if (!Directory.Exists(cacheRoot)) Directory.CreateDirectory(cacheRoot);
            var cachePath = Path.Combine(cacheRoot, className[..^"Controller".Length]);
            if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);
            return new FileCacheService<T>(cachePath);
        }

        public string FilePath(string key) => Path.Combine(cachePath, key);

        public bool Exists(string key) => File.Exists(FilePath(key));

        public async Task<bool> SetAsync(string key, byte[] content, bool overwrite = true)
        {
            if (!overwrite && Exists(key)) return false;
            try
            {
                await File.WriteAllBytesAsync(FilePath(key), content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SetTextAsync(string key, string content, bool overwrite = true) => await SetAsync(key, Encoding.UTF8.GetBytes(content), overwrite);

        public async Task<byte[]?> GetAsync(string key)
        {
            if (!Exists(key)) return null;
            try
            {
                return await File.ReadAllBytesAsync(FilePath(key));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> GetTextAsync(string key)
        {
            var content = await GetAsync(key);
            return content == null ? null : Encoding.UTF8.GetString(content);
        }
    }
}
