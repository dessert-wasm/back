using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Dessert.Utilities.Configuration
{
    public class YamlConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new YamlConfigurationProvider(this);
        }
    }

    public class YamlConfigurationProvider : FileConfigurationProvider
    {
        public YamlConfigurationProvider(YamlConfigurationSource source)
            : base(source) { }

        public override void Load(Stream stream)
        {
            try
            {
                Data = YamlConfigurationFileParser.Parse(stream);
            }
            catch (Exception e)
            {
                throw new FormatException("Error while parsing Yaml", e);
            }
        }
    }

    public static class YamlConfigurationSourceExtensions
    {
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
        }
        
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid path", nameof(path));
            }

            var yamlConfigurationSource = new YamlConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange,
            };
            yamlConfigurationSource.ResolveFileProvider();
            return builder.Add(yamlConfigurationSource);
        }
    }
}