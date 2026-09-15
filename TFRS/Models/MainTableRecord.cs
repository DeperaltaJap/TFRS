using System;
using System.ComponentModel.DataAnnotations;

namespace TFRS.Models
{
    public class MainTableRecord
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Date Submitted")]
        public string? DateSubmitted { get; set; }

        [Required(ErrorMessage = "Franchise number is required")]
        [Display(Name = "Tricycle Franchise Number")]
        public string? TricycleFranchiseNumber { get; set; }

        [Required(ErrorMessage = "Operator name is required")]
        [Display(Name = "Name of Operator")]
        public string? NameOfOperator { get; set; }

        public string? ContactNo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        public string? CommunityTaxNo { get; set; }
        public string? PlaceIssued { get; set; }

        [Required(ErrorMessage = "Make is required")]
        public string? Make { get; set; }

        [Required(ErrorMessage = "Year model is required")]
        [Range(1900, 2100, ErrorMessage = "Year model is invalid")]
        public int? YearModel { get; set; }

        public string? Color { get; set; }
        public string? EngineNumber { get; set; }

        public string? MVFileNo { get; set; }
        public string? ChassisNumber { get; set; }

        [Required(ErrorMessage = "Plate number is required")]
        public string? PlateNumber { get; set; }

        [Required(ErrorMessage = "Date issued is required")]
        [DataType(DataType.Date)]
        public DateTime? DateIssued { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateExpired { get; set; }

        public string? TODA { get; set; }
        public string? YearRenew { get; set; }
        public string? YearExpired { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }

        public string? Remarks { get; set; }

        public string? RouteCovered { get; set; }
        public string? CommitteeReport { get; set; }

        public string? SBApprovedDate { get; set; }

        public string? TypeOfOwnership { get; set; }
        public string? TypeOfApplication { get; set; }
        public string? VerifiedBy { get; set; }
    }
}
