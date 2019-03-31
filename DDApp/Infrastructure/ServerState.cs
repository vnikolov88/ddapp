using DDApp.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;

namespace DDApp.Infrastructure
{
    public class ServerState : IServerState
    {
        private ulong _viewsPackageVersion;
        private readonly IHostingEnvironment _env;

        public ServerState(IHostingEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));

            var viewsPath = $"{_env.ContentRootPath}\\Views";

            _viewsPackageVersion = CalculateFolderSize(viewsPath).GetGUID();
            ChangeToken.OnChange(
                () => _env.ContentRootFileProvider.Watch("Views\\**\\**\\*"),
                state => _viewsPackageVersion = CalculateFolderSize(viewsPath).GetGUID(),
                _env);
        }

        public ulong GetViewsPackageVersion()
        {
            return _viewsPackageVersion;
        }

        #region Auxiliary methods
        protected static float CalculateFolderSize(string folder)
        {
            var folderSize = 0.0f;
            try
            {
                //Checks if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
                
                try
                {
                    foreach (var file in Directory.GetFiles(folder))
                    {
                        if (!File.Exists(file)) continue;

                        FileInfo finfo = new FileInfo(file);
                        folderSize += finfo.Length;
                    }

                    foreach (var dir in Directory.GetDirectories(folder))
                        folderSize += CalculateFolderSize(dir);
                }
                catch (NotSupportedException e)
                {
                    Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
            }
            return folderSize;
        }
        #endregion Auxiliary methods
    }
}
