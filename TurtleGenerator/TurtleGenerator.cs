using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    /// <summary>
    /// Class that transforms given dictionary into turtle rdf 
    /// </summary>
    class TurtleGenerator
    {
        private ITurtleMapper _mapper;
        private StringBuilder _sb;

        public TurtleGenerator(ITurtleMapper mapper, StringBuilder sb)
        {
            _mapper = mapper;
            _sb = sb;
            _mapper.Init();
        }

        /// <summary>
        /// Transforms given dictionary to turtle rdf according to the mapper
        /// </summary>
        /// <param name="parsedFile">parsedFile</param>
        /// <param name="sb">output</param>
        public void TransformToTurtleRdf(Dictionary<int, Dictionary<string, string>> parsedFile)
        {
            int lineNumber = 1;

            _mapper.AddVocabularyDeclaration(_sb);

            foreach (Dictionary<string, string> line in parsedFile.Values)
            {
                Console.WriteLine("Processing entry {0} from {1} entries", lineNumber, parsedFile.Values.Count);

                _mapper.BeginNewEntry(_sb, lineNumber++);

                foreach (string columnKey in line.Keys)
                {
                    _mapper.MapEntry(columnKey, line[columnKey], _sb);
                }

                if (_sb.Length >= 3) //some magic
                    _sb.Replace(';', '.', _sb.Length - 3, 3);
            }
        }
    }
}
