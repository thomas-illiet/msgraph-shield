using System;

namespace MSGraphShield.Proxy.Plumbings.Pipeline
{
    /// <summary>
    /// Specifies the category and position of a pipeline step.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class PipelineAttribute : Attribute
    {
        /// <summary>
        /// Gets the category of the pipeline step.
        /// </summary>
        public PipelineCategory Category { get; }

        /// <summary>
        /// Gets the position of the pipeline step.
        /// </summary>
        public uint Position { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineAttribute"/> class
        /// with the specified category and position.
        /// </summary>
        /// <param name="category">The category of the pipeline step.</param>
        /// <param name="position">The position of the pipeline step.</param>
        public PipelineAttribute(PipelineCategory category, uint position)
        {
            Category = category;
            Position = position;
        }
    }
}