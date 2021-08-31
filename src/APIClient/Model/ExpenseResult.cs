/*
 * Clean Code API
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace APIClient.Model
{
    /// <summary>
    /// ExpenseResult
    /// </summary>
    [DataContract(Name = "ExpenseResult")]
    public partial class ExpenseResult : IEquatable<ExpenseResult>, IValidatableObject
    {

        /// <summary>
        /// Gets or Sets Product
        /// </summary>
        [DataMember(Name = "product", EmitDefaultValue = false)]
        public Product? Product { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpenseResult" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="product">product.</param>
        /// <param name="description">description.</param>
        /// <param name="price">price.</param>
        /// <param name="worker">worker.</param>
        public ExpenseResult(int id = default(int), Product? product = default(Product?), string description = default(string), double price = default(double), Worker worker = default(Worker))
        {
            this.Id = id;
            this.Product = product;
            this.Description = description;
            this.Price = price;
            this.Worker = worker;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        [DataMember(Name = "description", EmitDefaultValue = true)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets Price
        /// </summary>
        [DataMember(Name = "price", EmitDefaultValue = false)]
        public double Price { get; set; }

        /// <summary>
        /// Gets or Sets Worker
        /// </summary>
        [DataMember(Name = "worker", EmitDefaultValue = false)]
        public Worker Worker { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ExpenseResult {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Product: ").Append(Product).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  Price: ").Append(Price).Append("\n");
            sb.Append("  Worker: ").Append(Worker).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as ExpenseResult);
        }

        /// <summary>
        /// Returns true if ExpenseResult instances are equal
        /// </summary>
        /// <param name="input">Instance of ExpenseResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ExpenseResult input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Id == input.Id ||
                    this.Id.Equals(input.Id)
                ) && 
                (
                    this.Product == input.Product ||
                    this.Product.Equals(input.Product)
                ) && 
                (
                    this.Description == input.Description ||
                    (this.Description != null &&
                    this.Description.Equals(input.Description))
                ) && 
                (
                    this.Price == input.Price ||
                    this.Price.Equals(input.Price)
                ) && 
                (
                    this.Worker == input.Worker ||
                    (this.Worker != null &&
                    this.Worker.Equals(input.Worker))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                hashCode = hashCode * 59 + this.Id.GetHashCode();
                hashCode = hashCode * 59 + this.Product.GetHashCode();
                if (this.Description != null)
                    hashCode = hashCode * 59 + this.Description.GetHashCode();
                hashCode = hashCode * 59 + this.Price.GetHashCode();
                if (this.Worker != null)
                    hashCode = hashCode * 59 + this.Worker.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
