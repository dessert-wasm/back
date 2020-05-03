using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using YamlDotNet.RepresentationModel;

namespace Dessert.Utilities.Configuration
{
    internal class YamlConfigurationFileParser
    {
        private YamlConfigurationFileParser()
        {
        }

        private readonly IDictionary<string, string> _data =
            new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;

        public static IDictionary<string, string> Parse(Stream input)
        {
            return new YamlConfigurationFileParser().ParseStream(input);
        }

        public IDictionary<string, string> ParseStream(Stream stream)
        {
            _data.Clear();

            var document = new StreamReader(stream).ReadToEnd();
            var input = new StringReader(document);
            var yaml = new YamlStream();
            yaml.Load(input);

            VisitMapping(yaml.Documents[0].RootNode);

            return _data;
        }

        private void VisitMapping(YamlNode valueNode)
        {
            foreach (var item in ((YamlMappingNode) valueNode).Children)
            {
                string key = ((YamlScalarNode) item.Key).Value;
                EnterContext(key);
                VisitValue(item.Value);
                ExitContext();
            }
        }

        private void VisitValue(YamlNode valueNode)
        {
            switch (valueNode.NodeType)
            {
                case YamlNodeType.Alias:
                    break;
                case YamlNodeType.Mapping:
                    VisitMapping(valueNode);
                    break;
                case YamlNodeType.Scalar:
                    var key = _currentPath;
                    string value = ((YamlScalarNode) valueNode).Value;
                    _data[key] = value;
                    break;
                case YamlNodeType.Sequence:
                    var index = 0;
                    foreach (var item in ((YamlSequenceNode) valueNode).Children)
                    {
                        EnterContext(index.ToString());
                        VisitValue(item);
                        ExitContext();
                        index++;
                    }

                    break;
                default:
                    break;
            }
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}