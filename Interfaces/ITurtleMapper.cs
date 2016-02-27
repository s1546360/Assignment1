using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    /// <summary>
    /// Interface of a mapper used by the TurtleGenerator.
    /// Maps given values according to their column position (columnKey)
    /// </summary>
    public interface ITurtleMapper
    {               
        /// <summary>
        /// Init
        /// </summary>
        void Init();
        
        /// <summary>
        /// Adds the vocabulary declaration to the file
        /// </summary>
        /// <param name="sb"></param>
        void AddVocabularyDeclaration(StringBuilder sb);

        /// <summary>
        /// Begins a new entry in the turtle file by adding a new line
        /// </summary>
        /// <param name="sb"></param>
        void BeginNewEntry(StringBuilder sb, int line);

        /// <summary>
        /// Adds new entry to the output, transforms value into turtle syntax according to key
        /// </summary>
        void MapEntry(string columnKey, string value, StringBuilder sb);
    }
}
