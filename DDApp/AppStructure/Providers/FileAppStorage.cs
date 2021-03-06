﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DDApp.AppStructure.Providers
{
    public class FileAppStorage : IAppStorage
    {
        private readonly IHostingEnvironment _env;

        public FileAppStorage(IHostingEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public IChangeToken GetAppChangeToken(string appCode)
        {
            return _env.ContentRootFileProvider.Watch($"Apps/{appCode}/");
        }

        public IChangeToken GetAppModuleChangeToken(string moduleName)
        {
            return _env.ContentRootFileProvider.Watch($"Apps/global/{moduleName}_module.json");
        }

        public IEnumerable<string> GetAllAppCodes()
        {
            var contents = _env.ContentRootFileProvider.GetDirectoryContents("Apps/");
            return contents.Where(x => x.IsDirectory && Directory.GetFiles(x.PhysicalPath, $"{x.Name}.json").Any())
                .Select(x => x.Name);
        }
        public IEnumerable<string> GetAllAppModuleNames()
        {
            var contents = _env.ContentRootFileProvider.GetDirectoryContents("Apps/global/");
            return contents.Where(x => !x.IsDirectory && x.PhysicalPath.EndsWith("_module.json"))
                .Select(x => x.Name.Substring(0, x.Name.Length - "_module.json".Length)).ToList();
        }

        public async Task<string> GetAppAsync(string appCode)
        {
            var appKey = $"{appCode}.json";
            return await File.ReadAllTextAsync($"./Apps/{appCode}/{appKey}");
        }

        public async Task SetAppAsync(string appCode, string appContent)
        {
            var appKey = $"{appCode}.json";
            await File.WriteAllTextAsync($"./Apps/{appCode}/{appKey}", appContent);
        }

        public async Task<string> GetAppModuleAsync(string moduleName)
        {
            return await File.ReadAllTextAsync($"./Apps/global/{moduleName}_module.json");
        }
    }
}
