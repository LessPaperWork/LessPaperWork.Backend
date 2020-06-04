using LessPaper.Shared.Enums;

namespace LessPaper.Shared.Interfaces.General
{
    public interface ITag
    {
        /// <summary>
        /// Tag text
        /// </summary>
        string Value { get; }

        float Relevance { get; }

        TagSource Source { get; }
    }
}
