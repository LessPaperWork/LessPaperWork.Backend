using System;
using LessPaper.Shared.Enums;

namespace LessPaper.Shared.Helper
{
    /// <summary>
    /// Id generator
    /// </summary>
    public static class IdGenerator
    {
        /// <summary>
        /// Length of the id
        /// </summary>
        public const int IdLength = 34;

        /// <summary>
        /// Generates a new unique id
        /// </summary>
        /// <param name="idType">Target type of the id</param>
        /// <returns>Unique id</returns>
        public static string NewId(IdType idType)
        {
            var leadingId = ((int)idType).ToString("D2");
            var randomPart = Guid.NewGuid().ToString("N");
            return leadingId + randomPart;
        }

        /// <summary>
        /// Check if id is a specific type
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="targetIdType">Target type</param>
        /// <returns>True if type is matched</returns>
        public static bool IsType(string id, IdType targetIdType)
        {
            return id != null && TypeFromId(id, out var idType) && idType == targetIdType;
        }

        /// <summary>
        /// Determines the type of an id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="typeOfId">Type of the id</param>
        /// <returns>True if the type of the id could be resolved</returns>
        public static bool TypeFromId(string id, out IdType typeOfId)
        {
            if (id == null || id.Length != IdLength)
            {
                typeOfId = IdType.Undefined;
                return false;
            }
            
            // Get id character sequence and convert it to int
            var typeChars = id.Substring(0, 2);
            if (!int.TryParse(typeChars, out var typeNumber))
            {
                typeOfId = IdType.Undefined;
                return false;
            }

            // Check if the number is defined as enum value
            if (Enum.IsDefined(typeof(IdType), typeNumber))
            {
                typeOfId =  (IdType)typeNumber;
                return true;
            }
            
            typeOfId = IdType.Undefined;
            return false;
        }

    }
}
