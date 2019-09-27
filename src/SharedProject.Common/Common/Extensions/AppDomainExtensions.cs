﻿using System;
using System.IO;

namespace SimpleTrace.Common
{
    public static class AppDomainExtensions
    {
        public static string Combine(this AppDomain appDomain, params string[] paths)
        {
            if (paths.Length == 0)
            {
                return appDomain.BaseDirectory;
            }
            var combine = Path.Combine(paths);
            return Path.Combine(appDomain.BaseDirectory, combine);
        }
    }
}
