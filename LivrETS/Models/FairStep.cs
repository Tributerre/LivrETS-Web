using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivrETS.Models
{
    public class FairStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Fair))]
        public Guid FairID { get; set; }
        public virtual Fair Fair { get; set; }

        [NotMapped]
        public const string PICKING_CODE = "P";
        [NotMapped]
        public const string SALE_CODE = "S";
        [NotMapped]
        public const string RETRIEVAL_CODE = "R";

        [NotMapped]
        public const string PA_CODE = "PA";
        [NotMapped]
        public const string PB_CODE = "PB";
        [NotMapped]
        public const string PE_CODE = "PE";

        [Required]
        public string Place { get; set; }
        [Required]
        public string Phase { get; set; }
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        public DateTime EndDateTime { get; set; }

        [NotMapped]
        public string TypeNamePhase
        {
            get
            {
                switch (Phase)
                {
                    case PICKING_CODE:
                        return "Ceuillette";
                    case SALE_CODE:
                        return "Vente";
                    case RETRIEVAL_CODE:
                        return "Récupération";
                    default:
                        return string.Empty;
                }
            }
        }

        [NotMapped]
        public string TypeNamePlace
        {
            get
            {
                switch (Place)
                {
                    case PA_CODE:
                        return "Hall Pavillon A";
                    case PB_CODE:
                        return "Hall Pavillon B";
                    case PE_CODE:
                        return "Hall Pavillon E";
                    default:
                        return string.Empty;
                }
            }
        }

        public FairStep(string PhaseCode, string PlaceCode) 
        {
            this.Place = PlaceCode;
            this.Phase = PhaseCode;  
        }

        public FairStep(){}
    }

    
}