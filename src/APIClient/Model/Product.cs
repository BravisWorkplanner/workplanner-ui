/*
 * Clean Code API
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIClient.Model
{
    /// <summary>
    /// Defines Product
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Product
    {
        /// <summary>
        /// Enum Material for value: Material
        /// </summary>
        [EnumMember(Value = "Material")]
        Material = 1,

        /// <summary>
        /// Enum Tools for value: Tools
        /// </summary>
        [EnumMember(Value = "Tools")]
        Tools = 2,

        /// <summary>
        /// Enum MachineHiring for value: MachineHiring
        /// </summary>
        [EnumMember(Value = "MachineHiring")]
        MachineHiring = 3,

        /// <summary>
        /// Enum WorkerClothes for value: WorkerClothes
        /// </summary>
        [EnumMember(Value = "WorkerClothes")]
        WorkerClothes = 4

    }

}
